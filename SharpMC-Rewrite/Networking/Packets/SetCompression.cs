namespace SharpMCRewrite
{
	public class SetCompression : IPacket
	{
		public int PacketID
		{
			get { return 0x46; }
		}

		public bool IsPlayePacket
		{
			get { return true; }
		}

		public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
		}

		public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
			buffer.WriteVarInt(PacketID);
			buffer.WriteVarInt((int) Arguments[0]);
			buffer.FlushData();
		}
	}
}