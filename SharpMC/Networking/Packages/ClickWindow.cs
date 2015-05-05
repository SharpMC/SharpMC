// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	public class ClickWindow : Package<ClickWindow>
	{
		public ClickWindow(ClientWrapper client) : base(client)
		{
			ReadId = 0x0E;
		}

		public ClickWindow(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0E;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				byte WindowId = (byte)Buffer.ReadByte();
				short Slot = Buffer.ReadShort();
				byte Button = (byte) Buffer.ReadByte();
				short ActionNumber = Buffer.ReadShort();
				byte Mode = (byte) Buffer.ReadByte();

				short ItemId = Buffer.ReadShort();
				if (ItemId != -1)
				{
					byte ItemCount = (byte) Buffer.ReadByte();
					short ItemDamage = Buffer.ReadShort();
					byte Nbt = (byte) Buffer.ReadByte();
					if (Nbt != 0)
					{
						//NBT Data found
					}

					if (Client.Player.Inventory.GetSlot(Slot).ItemId == ItemId) //Is the information true?
					{
						Client.Player.Inventory.SetSlot(Slot, -1, 0 , 0);
						Client.Player.Inventory.ClickedItem = new ItemStack(ItemId, ItemCount, (byte) ItemDamage);
					}
				}
				else
				{
					var slot = Client.Player.Inventory.ClickedItem;
					Client.Player.Inventory.SetSlot(Slot, slot.ItemId, slot.MetaData, slot.ItemCount);
				}
			}
		}
	}
}
