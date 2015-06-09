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

using SharpMC.Core.Enums;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class SpawnObject : Package<SpawnObject>
	{
		public object Data;
		public int EntityId = 0;
		public byte Pitch = 0;
		public ObjectType Type;
		public short VelocityX = 0;
		public short VelocityY = 0;
		public short VelocityZ = 0;
		public double X = 0;
		public double Y = 0;
		public byte Yaw = 0;
		public double Z = 0;

		public SpawnObject(ClientWrapper client) : base(client)
		{
			SendId = 0x0E;
		}

		public SpawnObject(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0E;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteByte((byte) Type);
				Buffer.WriteInt((int) X*32);
				Buffer.WriteInt((int) Y*32);
				Buffer.WriteInt((int) Z*32);
				Buffer.WriteByte(Pitch);
				Buffer.WriteByte(Yaw);
				if (Type == ObjectType.ItemStack)
				{
					Buffer.WriteInt(0);
				}
				if (Type == ObjectType.ActivatedTnt)
				{
					Buffer.WriteInt(0);
				}
				if (Type == ObjectType.Snowball)
				{
					Buffer.WriteInt((int) Data);
					Buffer.WriteShort(0);
					Buffer.WriteShort(0);
					Buffer.WriteShort(0);
				}
				Buffer.FlushData();
			}
		}
	}
}