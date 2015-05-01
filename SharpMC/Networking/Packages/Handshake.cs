using System;
using System.Linq;
using System.Net;
using System.Text;
using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class Handshake : Package<Handshake>
	{
		public Handshake(ClientWrapper client) : base(client)
		{
			ReadId = 0x00;
			SendId = 0x00;
		}

		public Handshake(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x00;
			SendId = 0x00;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var protocol = Buffer.ReadVarInt();
				var host = Buffer.ReadString();
				var port = Buffer.ReadShort();
				var state = Buffer.ReadVarInt();

				switch (@state)
				{
					case 1:
						HandleStatusRequest();
						break;
					case 2:
						HandleLogin(protocol);
						break;
				}
			}
		}

		private void HandleStatusRequest()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString("{\"version\": {\"name\": \"" + Globals.ProtocolName + "\",\"protocol\": " +
				                   Globals.ProtocolVersion + "},\"players\": {\"max\": " + Globals.MaxPlayers + ",\"online\": " +
				                   Globals.Level.OnlinePlayers.Count + "},\"description\": {\"text\":\"" + Globals.RandomMOTD +
				                   "\"}}");
				Buffer.FlushData();
			}
		}

		private void HandleLogin(int protocol)
		{
			if (Buffer != null)
			{
				var usernameRaw = Buffer.ReadString();
				var username =
					new string(usernameRaw.Where(c => char.IsLetter(c) || char.IsPunctuation(c) || char.IsDigit(c)).ToArray());
				//username = Regex.Replace(username, @"[^\u0000-\u007F]", string.Empty);
				var uuid = getUUID(username);

				new LoginSucces(Client) {Username = username, UUID = uuid}.Write();

				if (Encoding.UTF8.GetBytes(username).Length == 0)
				{
					new Disconnect(Client) {Reason = "§4SharpMC\n§fSomething went wrong while decoding your username!"}.Write();
					return;
				}

				//Protocol check!
				if (protocol < Globals.ProtocolVersion) //Protocol to old?
				{
					new Disconnect(Client) { Reason = "§4SharpMC\n§fYour Minecraft version is to old!\nPlease update in order to play!" }.Write();
					return;
				}

				if (protocol > Globals.ProtocolVersion) //Protocol to new?
				{
					new Disconnect(Client) { Reason = "§4SharpMC\n§fThis server is not yet updated for your version of Minecraft!\nIn order to play you have to use " + Globals.MCProtocolName }.Write();
					return;
				}

				Client.Player = new Player(Globals.Level)
				{
					Uuid = uuid,
					Username = username,
					Wrapper = Client,
					Gamemode = Gamemode.Surival
				};
				Client.PlayMode = true;

				new SetCompression(Client).Write();

				new JoinGame(Client) {Player = Client.Player}.Write();
				new SpawnPosition(Client).Write();
				Client.StartKeepAliveTimer();
				Client.Player.SendChunksFromPosition();
			}
		}

		private string getUUID(string username)
		{
			try
			{
				var wc = new WebClient();
				var result = wc.DownloadString("https://api.mojang.com/users/profiles/minecraft/" + username);
				var _result = result.Split('"');
				if (_result.Length > 1)
				{
					var UUID = _result[3];
					return new Guid(UUID).ToString();
				}
				return Guid.NewGuid().ToString();
			}
			catch
			{
				return Guid.NewGuid().ToString();
			}
		}
	}
}