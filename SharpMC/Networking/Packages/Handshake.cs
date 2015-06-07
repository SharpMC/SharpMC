#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Networking.Packages
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Text;

	using SharpMC.Entity;
	using SharpMC.Enums;
	using SharpMC.Utils;

	public class Handshake : Package<Handshake>
	{
		public Handshake(ClientWrapper client)
			: base(client)
		{
			this.ReadId = 0x00;
			this.SendId = 0x00;
		}

		public Handshake(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			this.ReadId = 0x00;
			this.SendId = 0x00;
		}

		public override void Read()
		{
			if (this.Buffer != null)
			{
				var protocol = this.Buffer.ReadVarInt();
				var host = this.Buffer.ReadString();
				var port = this.Buffer.ReadShort();
				var state = this.Buffer.ReadVarInt();

				switch (@state)
				{
					case 1:
						this.HandleStatusRequest();
						break;
					case 2:
						this.HandleLogin(protocol);
						break;
				}
			}
		}

		private void HandleStatusRequest()
		{
			if (this.Buffer != null)
			{
				this.Buffer.WriteVarInt(this.SendId);
				this.Buffer.WriteString(
					"{\"version\": {\"name\": \"" + Globals.ProtocolName + "\",\"protocol\": " + Globals.ProtocolVersion
					+ "},\"players\": {\"max\": " + Globals.MaxPlayers + ",\"online\": " + Globals.GetOnlineCount()
					+ "},\"description\": {\"text\":\"" + Globals.CleanForJson(Globals.RandomMOTD) + "\"}}");
				this.Buffer.FlushData();
			}
		}

		private void HandleLogin(int protocol)
		{
			if (this.Buffer != null)
			{
				var usernameRaw = this.Buffer.ReadString();
				var username =
					new string(usernameRaw.Where(c => char.IsLetter(c) || char.IsPunctuation(c) || char.IsDigit(c)).ToArray());

				// username = Regex.Replace(username, @"[^\u0000-\u007F]", string.Empty);
				var uuid = this.getUUID(username);

				if (!Globals.Offlinemode)
				{
					if (Globals.EncryptionEnabled)
					{
						this.Client.PacketMode = PacketMode.Login;
						this.Client.Username = username;
						new EncryptionRequest(this.Client)
							{
								PublicKey = PacketCryptography.PublicKeyToAsn1(Globals.ServerKey), 
								VerificationToken = PacketCryptography.GetRandomToken()
							}.Write();
						return;
					}

					if (!this.Client.Player.IsAuthenticated())
					{
						new LoginSucces(this.Client) { Username = username, UUID = uuid }.Write();
						new Disconnect(this.Client) { Reason = "§4SharpMC\n§fAuthentication failed!" }.Write();
						return;
					}
				}

				new LoginSucces(this.Client) { Username = username, UUID = uuid }.Write();

				if (Encoding.UTF8.GetBytes(username).Length == 0)
				{
					new Disconnect(this.Client) { Reason = "§4SharpMC\n§fSomething went wrong while decoding your username!" }.Write();
					return;
				}

				// Protocol check!
				if (protocol < Globals.ProtocolVersion)
				{
					// Protocol to old?
					new Disconnect(this.Client)
						{
							Reason =
								"§4SharpMC\n§fYour Minecraft version is to old!\nPlease update in order to play!"
						}
						.Write();
					return;
				}

				if (protocol > Globals.ProtocolVersion)
				{
					// Protocol to new?
					new Disconnect(this.Client)
						{
							Reason =
								"§4SharpMC\n§fThis server is not yet updated for your version of Minecraft!\nIn order to play you have to use "
								+ Globals.MCProtocolName
						}.Write();
					return;
				}

				this.Client.Player = new Player(Globals.LevelManager.MainLevel)
					                     {
						                     Uuid = uuid, 
						                     Username = username, 
						                     Wrapper = this.Client, 
						                     Gamemode =
							                     Globals.LevelManager.MainLevel.DefaultGamemode
					                     };
				this.Client.PacketMode = PacketMode.Play;

				new SetCompression(this.Client).Write();

				new JoinGame(this.Client) { Player = this.Client.Player }.Write();
				new SpawnPosition(this.Client).Write();
				this.Client.StartKeepAliveTimer();
				this.Client.Player.InitializePlayer();
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