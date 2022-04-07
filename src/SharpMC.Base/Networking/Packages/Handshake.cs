using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class Handshake : Package<Handshake>
	{
		public Handshake(ClientWrapper client) : base(client)
		{
			ReadId = 0x00;
			SendId = 0x00;
		}

		public Handshake(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x00;
			SendId = 0x00;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var protocol = Buffer.ReadVarInt();
				var host = Buffer.ReadString();
				var port = Buffer.ReadShort();
				var state = Buffer.ReadVarInt();

				Client.Protocol = protocol;

				switch (@state)
				{
					case 1:
						Client.PacketMode = PacketMode.Status;
						return;
					case 2:
						Client.PacketMode = PacketMode.Login;
						break;
				}
			}
		}
	}
}