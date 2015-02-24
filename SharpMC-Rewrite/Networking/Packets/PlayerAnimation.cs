namespace SharpMCRewrite
{
	public class PlayerAnimation : IPacket
	{
		public int PacketID
		{
			get { return 0x0A; }
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
		}
	}
}