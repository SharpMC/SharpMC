using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class HeldItemChange : Package<HeldItemChange>
	{
		public HeldItemChange(ClientWrapper client) : base(client)
		{
			SendId = 0x0A;
			ReadId = 0x0A;
		}

		public HeldItemChange(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0A;
			ReadId = 0x0A;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var slot = (byte) Buffer.ReadShort();
				Client.Player.HeldItemChanged(slot);
			}
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteByte((byte) Client.Player.Inventory.CurrentSlot);
				Buffer.FlushData();
			}
		}
	}
}