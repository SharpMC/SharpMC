using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class Ping : Package<Ping>
	{
		public long Time = 0;
		public Ping(ClientWrapper client) : base(client)
		{
			ReadId = 0x01;
			SendId = 0x01;
		}

		public Ping(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x01;
			SendId = 0x01;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				if (Client.Player != null)
				{
					Client.UpdatePing();
				}

				var d = Buffer.ReadLong();
				new Ping(Client){Time = d}.Write();
			}
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteLong(Time);
				Buffer.FlushData();
			}
		}
	}
}