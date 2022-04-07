using Newtonsoft.Json;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class Disconnect : Package<Disconnect>
	{
		public McChatMessage Reason;

		public Disconnect(ClientWrapper client) : base(client)
		{
			SendId = 0x40;
		}

		public Disconnect(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x40;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				var msg = JsonConvert.SerializeObject(Reason);
				Buffer.WriteVarInt(Client.PacketMode == PacketMode.Login ? 0x00 : SendId);
				Buffer.WriteString(msg);
				Buffer.FlushData();
			}
		}
	}
}