using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class ClientSettings : Package<ClientSettings>
	{
		public ClientSettings(ClientWrapper client) : base(client)
		{
			ReadId = 0x16;
		}

		public ClientSettings(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x16;
		}

		public override void Read()
		{
			var locale = Buffer.ReadString();
			var viewDistance = (byte) Buffer.ReadByte();
			var chatFlags = Buffer.ReadVarInt();
			var chatColours = Buffer.ReadBool();
			var skinParts = (byte) Buffer.ReadByte();
			var mainHand = Buffer.ReadVarInt();

			Client.Player.Locale = locale;
			Client.Player.ViewDistance = viewDistance;
			Client.Player.ChatColours = chatColours;
			Client.Player.ChatFlags = chatFlags;
			Client.Player.SkinParts = skinParts;
			Client.Player.MainHand = mainHand;
		}
	}
}