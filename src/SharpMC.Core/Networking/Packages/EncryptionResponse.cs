// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;
using System.Linq;
using System.Net;
using SharpMC.Core.Entity;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class EncryptionResponse : Package<EncryptionResponse>
	{
		public EncryptionResponse(ClientWrapper client) : base(client)
		{
			ReadId = 0x01;
		}

		public EncryptionResponse(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x01;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var length = Buffer.ReadVarInt();
				var sharedsecret = Buffer.Read(length);

				length = Buffer.ReadVarInt();
				var verifytoken = Buffer.Read(length);

				Client.SharedKey = PacketCryptography.Decrypt(sharedsecret);

				var recv = PacketCryptography.GenerateAes((byte[]) Client.SharedKey.Clone());
				var send = PacketCryptography.GenerateAes((byte[]) Client.SharedKey.Clone());

				var packetToken = PacketCryptography.Decrypt(verifytoken);

				if (!packetToken.SequenceEqual(PacketCryptography.VerifyToken))
				{
					//Wrong token! :(
					ConsoleFunctions.WriteWarningLine("Wrong token!");
					return;
				}

				Client.Decrypter = recv.CreateDecryptor();
				Client.Encrypter = send.CreateEncryptor();

				Client.EncryptionEnabled = true;
				Client.Player = new Player(Globals.LevelManager.MainLevel)
				{
					Uuid = GetUuid(Client.Username),
					Username = Client.Username,
					Wrapper = Client,
					Gamemode = Globals.LevelManager.MainLevel.DefaultGamemode
				};

				if (Client.Player.IsAuthenticated())
				{
					new LoginSucces(Client) {Username = Client.Username, Uuid = Client.Player.Uuid}.Write();
					Client.PacketMode = PacketMode.Play;

					new SetCompression(Client).Write();

					new JoinGame(Client) {Player = Client.Player}.Write();
					new SpawnPosition(Client).Write();

					//Client.StartKeepAliveTimer();
					Client.Player.InitializePlayer();
				}
				else
				{
					new LoginSucces(Client) {Username = Client.Username, Uuid = Client.Player.Uuid}.Write();
					new Disconnect(Client) {Reason = "Authentication failed! Try restarting your client."}.Write();
				}
			}
		}

		private string GetUuid(string username)
		{
			try
			{
				var wc = new WebClient();
				var result = wc.DownloadString("https://api.mojang.com/users/profiles/minecraft/" + username);
				var _result = result.Split('"');
				if (_result.Length > 1)
				{
					var uuid = _result[3];
					return new Guid(uuid).ToString();
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