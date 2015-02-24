namespace SharpMCRewrite
{
	public class Ping : IPacket
	{
		public int PacketID
		{
			get { return 0x01; }
		}

		public bool IsPlayePacket
		{
			get { return false; }
		}

		public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
			state.SendData(buffer.BufferedData); //Echo the received packet back. :)
		}

		public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
		}
	}
}