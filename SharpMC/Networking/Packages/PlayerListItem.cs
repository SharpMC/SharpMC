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
using SharpMC.Enums;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class PlayerListItem : Package<PlayerListItem>
	{
		public int Action = 0;
		public Gamemode Gamemode;
		public string Username;
		public string UUID;

		public PlayerListItem(ClientWrapper client) : base(client)
		{
			SendId = 0x38;
		}

		public PlayerListItem(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x38;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(Action);
				Buffer.WriteVarInt(1);
				//foreach(Player player in Globals.Level.OnlinePlayers)
				//{
				Buffer.WriteUUID(new Guid(UUID));
				switch (Action)
				{
					case 0:
						Buffer.WriteString(Username);
						Buffer.WriteVarInt(0);
						Buffer.WriteVarInt((byte) Gamemode);
						Buffer.WriteVarInt(0);
						Buffer.WriteBool(false);
						break;
					case 4:
						break;
				}
				//}
				Buffer.FlushData();
			}
		}
	}
}