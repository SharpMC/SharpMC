using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class ClientSettings : Package<ClientSettings>
	{
		public ClientSettings(ClientWrapper client) : base(client)
		{
			ReadId = 0x15;
		}

		public ClientSettings(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x15;
		}

		public override void Read()
		{
			var Locale = Buffer.ReadString();
			var ViewDistance = (byte)Buffer.ReadByte();
			var ChatFlags = (byte)Buffer.ReadByte();
			var ChatColours = Buffer.ReadBool();
			var SkinParts = (byte)Buffer.ReadByte();

			Client.Player.Locale = Locale;
			Client.Player.ViewDistance = ViewDistance;
			Client.Player.ChatColours = ChatColours;
			Client.Player.ChatFlags = ChatFlags;
			Client.Player.SkinParts = SkinParts;
		}
	}
}
