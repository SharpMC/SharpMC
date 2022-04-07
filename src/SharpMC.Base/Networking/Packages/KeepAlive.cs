using System;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class KeepAlive : Package<KeepAlive>
	{
		public KeepAlive(ClientWrapper client) : base(client)
		{
			ReadId = 0x00;
			SendId = 0x00;
		}

		public KeepAlive(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x00;
			SendId = 0x00;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				var id = new Random().Next(0, 100);
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(id);
				Buffer.FlushData();
			}
		}
	}
}