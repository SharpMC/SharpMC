using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharpMC.Log;
using SharpMC.Network;
using SharpMC.Network.Events;
using SharpMC.Network.Packets;
using SharpMC.Network.Packets.Handshake;
using SharpMC.Network.Packets.Login;
using SharpMC.Network.Packets.Play;
using SharpMC.Network.Packets.Status;
using SharpMC.Util;

namespace SharpMC
{
	public class MCNetConnection : NetConnection
	{
		private static readonly ILogger Log = LogManager.GetLogger(typeof(MCNetConnection));

		private MinecraftServer Server { get; }
		private Player Player { get; set; }

		private Random Rnd { get; } = new Random();

		public MCNetConnection(MinecraftServer server, Direction direction, Socket socket)
			: base(direction, socket)
        {
            Server = server;
			OnConnectionClosed += OnConnectionClosedEvent;
		}

		private void OnConnectionClosedEvent(object sender, ConnectionClosedEventArgs e)
		{
			if (Player == null) return;

			if (Player.Level != null)
				Player.Level.RemovePlayer(Player);

			Player.Level = null;
		}

		protected override void HandlePacket(Packet packet)
		{
			if (packet == null)
			{
				return;
			}

			switch (ConnectionState)
			{
				case ConnectionState.Handshake:
					HandleHandshake(packet);
					break;
				case ConnectionState.Status:
					HandleStatus(packet);
					break;
				case ConnectionState.Login:
					HandleLogin(packet);
					break;
				case ConnectionState.Play:
					HandlePlay(packet);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void HandleHandshake(Packet packet)
		{
			if (packet is HandshakePacket)
			{
				HandshakePacket handshake = (HandshakePacket)packet;
				ConnectionState = handshake.NextState;
			}
		}

		private void HandleStatus(Packet packet)
		{
			if (packet is RequestPacket)
			{
				SendPacket(new ResponsePacket()
				{
					ResponseMsg = Server.Info.GetMotd()
				});
			}
			else if (packet is PingPacket)
			{
				SendPacket(packet);
				Stop();
			}
		}

		#region Login
		private void HandleLogin(Packet packet)
		{
			if (packet is LoginStartPacket)
			{
				HandleLoginStart((LoginStartPacket) packet);
			}
			else if (packet is EncryptionResponsePacket)
			{
				HandleEncryptionResponse((EncryptionResponsePacket) packet);
			}
		}

		private void HandleEncryptionResponse(EncryptionResponsePacket packet)
		{
			byte[] decrypted = Server.RsaEncryption.Decrypt(packet.VerifyToken);
			for (int i = 0; i < _encryptionVerification.Length; i++)
			{
				if (!decrypted[i].Equals(_encryptionVerification[i]))
				{
					Log.LogWarning("Encryption verification failed!");
					return;
				}
			}

			byte[] decryptedSharedSecret = Server.RsaEncryption.Decrypt(packet.SharedSecret);
			Array.Resize(ref decryptedSharedSecret, 16);

			InitEncryption(decryptedSharedSecret);

			if (!AuthMojang()) //TODO: Check if we are in online mode.
			{

			}

			ChangeToPlay();
		}

		private byte[] _encryptionVerification;
		private void HandleLoginStart(LoginStartPacket packet)
		{
			Player = Server.PlayerFactory.CreatePlayer(this, packet.Username);

			if (Server.RsaEncryption != null) //We use encryption
			{
				_encryptionVerification = new byte[4];
				Random r = new Random(Environment.TickCount);
				r.NextBytes(_encryptionVerification);

				SendPacket(new EncryptionRequestPacket()
				{
					ServerId = "",
					PublicKey = Server.RsaEncryption.PublicKey,
					VerifyToken = _encryptionVerification
				});
			}
			else
			{
				ChangeToPlay();
			}
		}

		private void ChangeToPlay()
		{
			SendPacket(new SetCompressionPacket()
			{
				Threshold = CompressionThreshold
			});
		
			CompressionEnabled = true;

			SendPacket(new LoginSuccessPacket()
			{
				Username = Player.Username,
				UUID = Player.UUID.ToString()
			});
			ConnectionState = ConnectionState.Play;

			Player.InitiateGame();
		}

		private bool AuthMojang()
		{
			string serverHash;
			using (MemoryStream ms = new MemoryStream())
			{
				byte[] ascii = Encoding.ASCII.GetBytes("");
				ms.Write(ascii,	0, ascii.Length);
				ms.Write(SharedSecret, 0, 16);
				ms.Write(Server.RsaEncryption.PublicKey, 0, Server.RsaEncryption.PublicKey.Length);

				serverHash = JavaHexDigest(ms.ToArray());
			}

			using (WebClient wc = new WebClient())
			{
				string r = wc.DownloadString(string.Format("https://sessionserver.mojang.com/session/minecraft/hasJoined?username={0}&serverId={1}", Player.Username, serverHash));
				if (r.Length <= 10) return false;

				AuthResponse auth = JsonConvert.DeserializeObject<AuthResponse>(r);
				Player.Username = auth.Name;
				Player.DisplayName = auth.Name;
				Player.UUID = new Guid(auth.ID);
				Player.AuthResponse = auth;
			}
			return true;
		}

		public sealed class AuthResponse
		{
			[JsonProperty("id")]
			public string ID;

			[JsonProperty("name")]
			public string Name;

			[JsonProperty("properties")]
			public Property[] Properties;

			public struct Property
			{
				[JsonProperty("name")]
				public string Name;

				[JsonProperty("value")]
				public string Value;

				[JsonProperty("signature")]
				public string Signature;
			}
		}

		private static string JavaHexDigest(byte[] input)
		{
			var sha1 = SHA1.Create();
			byte[] hash = sha1.ComputeHash(input);
			bool negative = (hash[0] & 0x80) == 0x80;
			if (negative) // check for negative hashes
				hash = TwosCompliment(hash);
			// Create the string and trim away the zeroes
			string digest = GetHexString(hash).TrimStart('0');
			if (negative)
				digest = "-" + digest;
			return digest;
		}

		private static string GetHexString(byte[] p)
		{
			string result = string.Empty;
			for (int i = 0; i < p.Length; i++)
				result += p[i].ToString("x2"); // Converts to hex string
			return result;
		}

		private static byte[] TwosCompliment(byte[] p) // little endian
		{
			int i;
			bool carry = true;
			for (i = p.Length - 1; i >= 0; i--)
			{
				p[i] = (byte)~p[i];
				if (carry)
				{
					carry = p[i] == 0xFF;
					p[i]++;
				}
			}
			return p;
		}

		#endregion

		private void HandlePlay(Packet packet)
		{
			if (packet is KeepAlivePacket)
			{
				HandleKeepAlive((KeepAlivePacket) packet);
			}
			else if (packet is PlayerPosition)
			{
				HandlePlayerPos((PlayerPosition) packet);
			}
			else if (packet is PlayerPositionAndLookPacket)
			{
				HandlePlayerPosAndLook((PlayerPositionAndLookPacket) packet);
			}
			else if (packet is PlayerLookPacket)
			{
				HandlePlayerLook((PlayerLookPacket) packet);
			}
			else if (packet is ClientSettingsPacket)
			{
				HandleClientSettings((ClientSettingsPacket) packet);
			}
			else if (packet is ChatMessagePacket)
			{
				HandleChatMessage((ChatMessagePacket) packet);
			}
		}

		private void HandleChatMessage(ChatMessagePacket packet)
		{
			Player.Level.RelayBroadcast(new ChatMessagePacket()
			{
				Message = ChatHelper.EncodeChatMessage($"<{Player.Username}> {packet.Message}")
			});
		}

		private void HandleClientSettings(ClientSettingsPacket packet)
		{
			Player.ViewDistance = Math.Min((int) packet.ViewDistance, 12);
		}

		private void HandlePlayerLook(PlayerLookPacket packet)
		{
			Player.KnownPosition.Yaw = packet.Yaw;
			Player.KnownPosition.Pitch = packet.Pitch;
		}

		private void HandlePlayerPosAndLook(PlayerPositionAndLookPacket packet)
		{
			Player.KnownPosition.X = (float) packet.X;
			Player.KnownPosition.Y = (float) (packet.Y + 1.62f);
			Player.KnownPosition.Z = (float) packet.Z;
			Player.KnownPosition.Yaw = packet.Yaw;
			Player.KnownPosition.Pitch = packet.Pitch;
		}

		private void HandlePlayerPos(PlayerPosition packet)
		{
			Player.KnownPosition.X = (float) packet.X;
			Player.KnownPosition.Y = (float) (packet.FeetY + 1.62f);
			Player.KnownPosition.Z = (float) packet.Z;
		}

		public bool KeepAliveReady { get; private set; } = true;
		private int _lastKeepAlive;
		private void HandleKeepAlive(KeepAlivePacket packet)
		{
			if (packet.KeepAliveid.Equals(_lastKeepAlive))
			{
				KeepAliveReady = true;
			}
		}

		public void SendKeepAlive()
		{
			KeepAliveReady = false;
			_lastKeepAlive = Rnd.Next();
			SendPacket(new KeepAlivePacket() {KeepAliveid = _lastKeepAlive});
		}
	}
}
