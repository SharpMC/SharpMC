using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	public class CloseWindow : Package<CloseWindow>
	{
		public byte WindowId = 0;
		public CloseWindow(ClientWrapper client) : base(client)
		{
			ReadId = 0x0D;
		}

		public CloseWindow(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0D;
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
