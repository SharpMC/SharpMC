using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class Handshake : Package<Handshake>
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
			int protocol = Buffer.ReadVarInt();
			string host = Buffer.ReadString();
			short port = Buffer.ReadShort();
			int state = Buffer.ReadVarInt();

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
			Buffer.WriteString("{\"version\": {\"name\": \"" + Globals.ProtocolName + "\",\"protocol\": " + Globals.ProtocolVersion + "},\"players\": {\"max\": " + Globals.MaxPlayers + ",\"online\": " + Globals.Level.OnlinePlayers.Count + "},\"description\": {\"text\":\"" + Globals.RandomMOTD + "\"}}");
			Buffer.FlushData();
		}

		private void HandleLogin()
		{
			string username = Buffer.ReadUsername();
			string uuid = getUUID(username);

			new LoginSucces(Client) {Username = username, UUID = uuid}.Write();

			if (Encoding.UTF8.GetBytes(username).Length == 0)
			{
				new Disconnect(Client){Reason = "Something went wrong while decoding your username!"}.Write();
				return;
			}

			Globals.LastUniqueID++;
			Client.Player = new Player() { UUID = uuid, Username = username, UniqueServerID = Globals.LastUniqueID, Wrapper = Client, Gamemode = Gamemode.Creative };
			Client.PlayMode = true;

			if (!Globals.UseCompression)
				new SetCompression().Write(Client, Buffer, new object[] { -1 }); //Turn off compression.

			new JoinGame().Write(Client, Buffer, new object[] { Client.Player });
			new SpawnPosition().Write(Client, Buffer, new object[0]);
			Client.StartKeepAliveTimer();
			Client.Player.SendChunksFromPosition();
		}

		private string getUUID(string username)
		{
			try
			{
				WebClient wc = new WebClient();
				string result = wc.DownloadString("https://api.mojang.com/users/profiles/minecraft/" + username);
				string[] _result = result.Split('"');
				if (_result.Length > 1)
				{
					string UUID = _result[3];
					return new Guid(UUID).ToString();
				}
				else
				{
					return Guid.NewGuid().ToString();
				}
			}
			catch
			{
				return Guid.NewGuid().ToString();
			}
		}
	}
}
