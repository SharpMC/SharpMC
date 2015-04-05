using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Enums;

namespace SharpMCRewrite.Networking.Packages
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
					HandleLogin();
					break;
			}
		}

		private void HandleStatusRequest()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteString("{\"version\": {\"name\": \"" + Globals.ProtocolName + "\",\"protocol\": " +
			                   Globals.ProtocolVersion + "},\"players\": {\"max\": " + Globals.MaxPlayers + ",\"online\": " +
			                   Globals.Level.OnlinePlayers.Count + "},\"description\": {\"text\":\"" + Globals.RandomMOTD +
			                   "\"}}");
			Buffer.FlushData();
		}

		private void HandleLogin()
		{
			var usernameRAW = Buffer.ReadString();
			string username = new string(usernameRAW.Where(c => char.IsLetter(c) || char.IsPunctuation(c) || char.IsDigit(c)).ToArray());
			//username = Regex.Replace(username, @"[^\u0000-\u007F]", string.Empty);
			var uuid = getUUID(username);

			new LoginSucces(Client) {Username = username, UUID = uuid}.Write();

			if (Encoding.UTF8.GetBytes(username).Length == 0)
			{
				new Disconnect(Client) {Reason = "§4SharpMC\n§fSomething went wrong while decoding your username!"}.Write();
				return;
			}

			Globals.LastUniqueID++;
			Client.Player = new Player(Globals.Level)
			{
				Uuid = uuid,
				Username = username,
				UniqueServerId = Globals.LastUniqueID,
				Wrapper = Client,
				Gamemode = Gamemode.Creative
			};
			Client.PlayMode = true;

			new SetCompression(Client).Write();

			new JoinGame(Client){Player = Client.Player}.Write();
			new SpawnPosition(Client).Write();
			Client.StartKeepAliveTimer();
			Client.Player.SendChunksFromPosition();
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