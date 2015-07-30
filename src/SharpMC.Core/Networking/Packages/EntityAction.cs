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

using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class EntityAction : Package<EntityAction>
	{
		public EntityAction(ClientWrapper client) : base(client)
		{
			ReadId = 0x0B;
		}

		public EntityAction(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0B;
		}

		public override void Read()
		{
			var entityId = Buffer.ReadVarInt();
			var actionId = Buffer.ReadVarInt();
			var jumpBoost = Buffer.ReadVarInt();
			
			Client.Player.LastEntityAction = (Enums.EntityAction) actionId;
			if (actionId == 0)
			{
				Client.Player.IsCrouching = true;
			}
			else if (actionId == 1)
			{
				Client.Player.IsCrouching = false;
			}
		}
	}
}