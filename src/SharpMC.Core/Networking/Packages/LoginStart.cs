using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Core.Entity;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class LoginStart : Package<LoginStart>
	{
		public LoginStart(ClientWrapper client) : base(client)
		{
			ReadId = 0x00;
		}

		public LoginStart(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x00;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var usernameRaw = Buffer.ReadString();
				var username =
					new string(usernameRaw.Where(c => char.IsLetter(c) || char.IsPunctuation(c) || char.IsDigit(c)).ToArray());
				
				var uuid = getUUID(username);
				if (ServerSettings.OnlineMode)
				{
					if (ServerSettings.EncryptionEnabled)
					{
						Client.PacketMode = PacketMode.Login;
						Client.Username = username;
						new EncryptionRequest(Client)
						{
							PublicKey = PacketCryptography.PublicKeyToAsn1(Globals.ServerKey),
							VerificationToken = PacketCryptography.GetRandomToken()
						}.Write();
						return;
					}
					if (!Client.Player.IsAuthenticated())
					{
						new LoginSucces(Client) { Username = username, UUID = uuid }.Write();
						new Disconnect(Client) { Reason = "§4SharpMC\n§fAuthentication failed!" }.Write();
						return;
					}
				}

				new LoginSucces(Client) { Username = username, UUID = uuid }.Write();

				if (Encoding.UTF8.GetBytes(username).Length == 0)
				{
					new Disconnect(Client) { Reason = "§4SharpMC\n§fSomething went wrong while decoding your username!" }.Write();
					return;
				}

				//Protocol check!
				/*if (protocol < Globals.ProtocolVersion) //Protocol to old?
				{
					new Disconnect(Client) { Reason = "§4SharpMC\n§fYour Minecraft version is to old!\nPlease update in order to play!" }
						.Write();
					return;
				}

				if (protocol > Globals.ProtocolVersion) //Protocol to new?
				{
					new Disconnect(Client)
					{
						Reason =
							"§4SharpMC\n§fThis server is not yet updated for your version of Minecraft!\nIn order to play you have to use " +
							Globals.OfficialProtocolName
					}.Write();
					return;
				}*/

				Client.Player = new Player(Globals.LevelManager.MainLevel)
				{
					Uuid = uuid,
					Username = username,
					Wrapper = Client,
					Gamemode = Globals.LevelManager.MainLevel.DefaultGamemode
				};
				Client.PacketMode = PacketMode.Play;

				new SetCompression(Client).Write();

				new JoinGame(Client) { Player = Client.Player }.Write();
				new SpawnPosition(Client).Write();
				Client.StartKeepAliveTimer();
				Client.Player.InitializePlayer();
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
