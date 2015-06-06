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
using SharpMC.Entity;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class EntityRelativeMove : Package<EntityRelativeMove>
	{
		public Vector3 Movement;
		public Player Player;

		public EntityRelativeMove(ClientWrapper client) : base(client)
		{
			SendId = 0x15;
		}

		public EntityRelativeMove(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x15;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(Player.EntityId);
				Buffer.WriteByte((byte) (Movement.X*32));
				Buffer.WriteByte((byte) (Movement.Y*32));
				Buffer.WriteByte((byte) (Movement.Z*32));
				Buffer.WriteBool(Player.KnownPosition.OnGround);
				Buffer.FlushData();
			}
		}
	}
}