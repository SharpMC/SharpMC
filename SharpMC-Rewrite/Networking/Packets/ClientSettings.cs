namespace SharpMCRewrite
{
	public class ClientSettings : IPacket
	{
		public int PacketID
		{
			get { return 0x15; }
		}

		public bool IsPlayePacket
		{
			get { return true; }
		}

		public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
			var Locale = buffer.ReadString();
			var ViewDistance = (byte) buffer.ReadByte();
			var ChatFlags = (byte) buffer.ReadByte();
			var ChatColours = buffer.ReadBool();
			var SkinParts = (byte) buffer.ReadByte();

			state.Player.Locale = Locale;
			state.Player.ViewDistance = ViewDistance;
			state.Player.ChatColours = ChatColours;
			state.Player.ChatFlags = ChatFlags;
			state.Player.SkinParts = SkinParts;
		}

		public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
		}
	}
}