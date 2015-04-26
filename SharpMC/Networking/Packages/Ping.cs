using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class Ping : Package<Ping>
	{
		public Ping(ClientWrapper client) : base(client)
		{
			ReadId = 0x01;
		}

		public Ping(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x01;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				Client.SendData(Buffer.BufferedData);
			}
		}
	}
}