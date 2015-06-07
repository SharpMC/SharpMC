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

	using SharpMC.Entity;
	using SharpMC.Enums;
	using SharpMC.Utils;

	public class EncryptionResponse : Package<EncryptionResponse>
	{
		public EncryptionResponse(ClientWrapper client)
			: base(client)
		{
			this.ReadId = 0x01;
		}

		public EncryptionResponse(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			this.ReadId = 0x01;
		}

		public override void Read()
		{
			if (this.Buffer != null)
			{
				var length = this.Buffer.ReadVarInt();
				var sharedsecret = this.Buffer.Read(length);

				length = this.Buffer.ReadVarInt();
				var verifytoken = this.Buffer.Read(length);

				this.Client.SharedKey = PacketCryptography.Decrypt(sharedsecret);

				var recv = PacketCryptography.GenerateAES((byte[])this.Client.SharedKey.Clone());
				var send = PacketCryptography.GenerateAES((byte[])this.Client.SharedKey.Clone());

				var packetToken = PacketCryptography.Decrypt(verifytoken);

				if (!packetToken.SequenceEqual(PacketCryptography.VerifyToken))
				{
					// Wrong token! :(
					ConsoleFunctions.WriteWarningLine("Wrong token!");
					return;
				}

				this.Client.Decrypter = recv.CreateDecryptor();
				this.Client.Encrypter = send.CreateEncryptor();

				this.Client.EncryptionEnabled = true;
				this.Client.Player = new Player(Globals.LevelManager.MainLevel)
					                     {
						                     Uuid = this.getUUID(this.Client.Username), 
						                     Username = this.Client.Username, 
						                     Wrapper = this.Client, 
						                     Gamemode =
							                     Globals.LevelManager.MainLevel.DefaultGamemode
					                     };

				if (this.Client.Player.IsAuthenticated())
				{
					new LoginSucces(this.Client) { Username = this.Client.Username, UUID = this.Client.Player.Uuid }.Write();
					this.Client.PacketMode = PacketMode.Play;

					new SetCompression(this.Client).Write();

					new JoinGame(this.Client) { Player = this.Client.Player }.Write();
					new SpawnPosition(this.Client).Write();

					this.Client.StartKeepAliveTimer();
					this.Client.Player.InitializePlayer();
				}
				else
				{
					new LoginSucces(this.Client) { Username = this.Client.Username, UUID = this.Client.Player.Uuid }.Write();
					new Disconnect(this.Client) { Reason = "Authentication failed! Try restarting your client." }.Write();
				}
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