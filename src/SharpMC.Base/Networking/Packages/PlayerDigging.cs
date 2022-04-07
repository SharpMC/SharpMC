// Distributed under the MIT license
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

using System.Numerics;
using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.Util;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerDigging : Package<PlayerDigging>
	{
		public PlayerDigging(ClientWrapper client) : base(client)
		{
			ReadId = 0x07;
		}

		public PlayerDigging(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x07;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var status = Buffer.ReadByte();
				var position = Buffer.ReadPosition();
				var face = Buffer.ReadByte();

				if (status == 3)
				{
					//Drop item stack
					Client.Player.Inventory.DropCurrentItemStack();
					return;
				}

				if (status == 4)
				{
					//Drop item
					Client.Player.Inventory.DropCurrentItem();
					return;
				}

				if (status == 6)
				{
					Client.Player.Inventory.SwapHands();
					return;
				}

				if (position == Vector3.Zero) return;

				if (status == 2 || Client.Player.Gamemode == Gamemode.Creative)
				{
					var block = Client.Player.Level.GetBlock(position);
					block.BreakBlock(Client.Player.Level);
					Client.Player.Digging = false;

					if (Client.Player.Gamemode != Gamemode.Creative)
					{
						foreach (var its in block.Drops)
						{
							new ItemEntity(Client.Player.Level, its)
							{
								KnownPosition = new PlayerLocation(position.X, position.Y, position.Z)
							}.SpawnEntity();
						}
					}
				}

				else if (status == 0)
				{
					Client.Player.Digging = true;
				}
				else if (status == 1)
				{
					Client.Player.Digging = false;
				}
			}
		}
	}
}