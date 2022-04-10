using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharpMC.Logging;
using SharpMC.Network;
using SharpMC.Network.Events;
using SharpMC.Network.Packets;
using SharpMC.Network.Packets.Handshaking.ToServer;
using SharpMC.Network.Packets.Login.ToClient;
using SharpMC.Network.Packets.Login.ToServer;
using SharpMC.Network.Packets.Play.ToBoth;
using SharpMC.Network.Packets.Play.ToServer;
using SharpMC.Network.Packets.Status.ToBoth;
using SharpMC.Network.Packets.Status.ToServer;
using SharpMC.Players;
using SharpMC.Util;
using EncryptionBegin = SharpMC.Network.Packets.Login.ToServer.EncryptionBegin;

namespace SharpMC.Net
{
    public class McNetConnection : NetConnection
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(McNetConnection));

        private MinecraftServer Server { get; }
        private Player Player { get; set; }
        private Random Rnd { get; } = new Random();

        public McNetConnection(MinecraftServer server, Direction direction, Socket socket)
            : base(direction, socket)
        {
            Server = server;
            OnConnectionClosed += OnConnectionClosedEvent;
        }

        private void OnConnectionClosedEvent(object sender, ConnectionClosedArgs e)
        {
            if (Player == null)
                return;
            Player.Level?.RemovePlayer(Player);
            Player.Level = null;
        }

        protected override void HandlePacket(Packet packet)
        {
            if (packet == null)
                return;
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
            if (packet is SetProtocol handshake)
            {
                ConnectionState = (ConnectionState) handshake.NextState;
            }
        }

        private void HandleStatus(Packet packet)
        {
            if (packet is PingStart)
            {
                SendPacket(new Network.Packets.Status.ToClient.ServerInfo
                {
                    Response = Server.Info.GetMotd()
                });
            }
            else if (packet is Ping)
            {
                SendPacket(packet);
                Stop();
            }
        }

        #region Login
        private void HandleLogin(Packet packet)
        {
            if (packet is LoginStart start)
            {
                HandleLoginStart(start);
            }
            else if (packet is EncryptionBegin begin)
            {
                HandleEncryptionResponse(begin);
            }
        }

        private byte[] _encryptionVerification;

        private void HandleEncryptionResponse(EncryptionBegin packet)
        {
            var decrypted = Server.RsaEncryption.Decrypt(packet.VerifyToken);
            for (var i = 0; i < _encryptionVerification.Length; i++)
            {
                if (!decrypted[i].Equals(_encryptionVerification[i]))
                {
                    Log.LogWarning("Encryption verification failed!");
                    return;
                }
            }
            var decryptedSharedSecret = Server.RsaEncryption.Decrypt(packet.SharedSecret);
            Array.Resize(ref decryptedSharedSecret, 16);
            InitEncryption(decryptedSharedSecret);
            if (!AuthMojang())
            {
                // TODO: Check if we are in online mode
            }
            ChangeToPlay();
        }

        private void HandleLoginStart(LoginStart packet)
        {
            Player = Server.PlayerFactory.CreatePlayer(this, packet.Username);
            if (Server.RsaEncryption != null)
            {
                _encryptionVerification = new byte[4];
                var r = new Random(Environment.TickCount);
                r.NextBytes(_encryptionVerification);
                // We use encryption
                SendPacket(new Network.Packets.Login.ToClient.EncryptionBegin
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
            SendPacket(new Compress
            {
                Threshold = CompressionThreshold
            });
            CompressionEnabled = true;
            SendPacket(new Success
            {
                Username = Player.Username,
                Uuid = Player.Uuid
            });
            ConnectionState = ConnectionState.Play;
            Player.InitiateGame();
        }

        private bool AuthMojang()
        {
            string serverHash;
            using (var ms = new MemoryStream())
            {
                var ascii = Encoding.ASCII.GetBytes("");
                ms.Write(ascii, 0, ascii.Length);
                ms.Write(SharedSecret, 0, 16);
                ms.Write(Server.RsaEncryption.PublicKey, 0, Server.RsaEncryption.PublicKey.Length);
                serverHash = JavaHexDigest(ms.ToArray());
            }
            using (var wc = new WebClient())
            {
                const string moj = "https://sessionserver.mojang.com/session/minecraft/hasJoined";
                const string main = moj + "?username={0}&serverId={1}";
                var url = string.Format(main, Player.Username, serverHash);
                var r = wc.DownloadString(url);
                if (r.Length <= 10)
                    return false;
                var auth = JsonConvert.DeserializeObject<AuthResponse>(r);
                Player.Username = auth.Name;
                Player.DisplayName = auth.Name;
                Player.Uuid = new Guid(auth.Id);
                Player.AuthResponse = auth;
            }
            return true;
        }

        private static string JavaHexDigest(byte[] input)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(input);
            var negative = (hash[0] & 0x80) == 0x80;
            if (negative) // check for negative hashes
                hash = TwosCompliment(hash);
            // Create the string and trim away the zeroes
            var digest = GetHexString(hash).TrimStart('0');
            if (negative)
                digest = "-" + digest;
            return digest;
        }

        private static string GetHexString(byte[] p)
        {
            var result = string.Empty;
            foreach (var t in p)
                result += t.ToString("x2"); // Converts to hex string
            return result;
        }

        private static byte[] TwosCompliment(byte[] p) // little endian
        {
            int i;
            var carry = true;
            for (i = p.Length - 1; i >= 0; i--)
            {
                p[i] = (byte) ~p[i];
                if (carry)
                {
                    carry = p[i] == 0xFF;
                    p[i]++;
                }
            }
            return p;
        }
        #endregion

        #region Play
        private void HandlePlay(Packet packet)
        {
            if (packet is KeepAlive keepAlive)
            {
                HandleKeepAlive(keepAlive);
            }
            else if (packet is Position)
            {
                HandlePlayerPos((Position) packet);
            }
            else if (packet is PositionLook)
            {
                HandlePlayerPosAndLook((PositionLook) packet);
            }
            else if (packet is Look)
            {
                HandlePlayerLook((Look) packet);
            }
            else if (packet is Settings)
            {
                HandleClientSettings((Settings) packet);
            }
            else if (packet is Chat)
            {
                HandleChatMessage((Chat) packet);
            }
        }

        private void HandleChatMessage(Chat packet)
        {
            Player.Level.RelayBroadcast(new Chat
            {
                Message = ChatHelper.EncodeChatMessage($"<{Player.Username}> {packet.Message}")
            });
        }

        private void HandleClientSettings(Settings packet)
        {
            Player.ViewDistance = Math.Min((int) packet.ViewDistance, 12);
        }

        private void HandlePlayerLook(Look packet)
        {
            Player.KnownPosition.Yaw = packet.Yaw;
            Player.KnownPosition.Pitch = packet.Pitch;
        }

        private void HandlePlayerPosAndLook(PositionLook packet)
        {
            Player.KnownPosition.X = (float) packet.X;
            Player.KnownPosition.Y = (float) (packet.Y + 1.62f);
            Player.KnownPosition.Z = (float) packet.Z;
            Player.KnownPosition.Yaw = packet.Yaw;
            Player.KnownPosition.Pitch = packet.Pitch;
        }

        private void HandlePlayerPos(Position packet)
        {
            Player.KnownPosition.X = (float) packet.X;
            Player.KnownPosition.Y = (float) (packet.Y + 1.62f);
            Player.KnownPosition.Z = (float) packet.Z;
        }

        public bool KeepAliveReady { get; private set; } = true;

        private int _lastKeepAlive;

        private void HandleKeepAlive(KeepAlive packet)
        {
            if (packet.KeepAliveId.Equals(_lastKeepAlive))
            {
                KeepAliveReady = true;
            }
        }

        public void SendKeepAlive()
        {
            KeepAliveReady = false;
            _lastKeepAlive = Rnd.Next();
            SendPacket(new KeepAlive {KeepAliveId = _lastKeepAlive});
        }
        #endregion
    }
}