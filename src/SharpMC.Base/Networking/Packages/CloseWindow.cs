using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class CloseWindow : Package<CloseWindow>
	{
		public byte WindowId = 0;
		public CloseWindow(ClientWrapper client) : base(client)
		{
			ReadId = 0x0E;
		}

		public CloseWindow(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0E;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				WindowId = (byte)Buffer.ReadByte();
				if (WindowId == 0)
				{
					Client.Player.Inventory.InventoryClosed();
				}
			}
		}
	}
}
