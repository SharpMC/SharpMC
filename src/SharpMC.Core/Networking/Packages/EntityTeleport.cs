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

using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Networking.Packages
{
	internal class EntityTeleport : Package<EntityTeleport>
	{
		public Vector3 Coordinates;
		public bool OnGround;
		public double Pitch;
		public int UniqueServerId;
		public double Yaw;

		public EntityTeleport(ClientWrapper client) : base(client)
		{
			SendId = 0x18;
		}

		public EntityTeleport(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x18;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(UniqueServerId);
				Buffer.WriteInt((int) Coordinates.X*32);
				Buffer.WriteInt((int) Coordinates.Y*32);
				Buffer.WriteInt((int) Coordinates.Z*32);
				Buffer.WriteByte((byte)((Yaw / 360) * 256));
				Buffer.WriteByte((byte)Pitch);
				Buffer.WriteBool(OnGround);
				Buffer.FlushData();
			}
		}
	}
}