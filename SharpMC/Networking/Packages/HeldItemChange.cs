using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class HeldItemChange : Package<HeldItemChange>
	{
		public HeldItemChange(ClientWrapper client) : base(client)
		{
			SendId = 0x09;
			ReadId = 0x09;
		}

		public HeldItemChange(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x09;
			ReadId = 0x09;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var slot = (byte) Buffer.ReadShort();
				//Client.Player.Inventory.CurrentSlot = slot;
				Client.Player.HeldItemChanged(slot);
			}
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteByte((byte)Client.Player.Inventory.CurrentSlot);
				Buffer.FlushData();
			}
		}
	}
}