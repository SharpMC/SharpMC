#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Networking.Packages
{
	using SharpMC.Entity;
	using SharpMC.Utils;
	using SharpMC.Worlds;

	internal class EntityTeleport : Package<EntityTeleport>
	{
		public Vector3 Coordinates;

		public bool OnGround;

		public byte Pitch;

		public int UniqueServerID;

		public byte Yaw;

		public EntityTeleport(ClientWrapper client)
			: base(client)
		{
			this.SendId = 0x18;
		}

		public EntityTeleport(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			this.SendId = 0x18;
		}

		public override void Write()
		{
			if (this.Buffer != null)
			{
				this.Buffer.WriteVarInt(this.SendId);
				this.Buffer.WriteVarInt(this.UniqueServerID);
				this.Buffer.WriteInt((int)this.Coordinates.X * 32);
				this.Buffer.WriteInt((int)this.Coordinates.Y * 32);
				this.Buffer.WriteInt((int)this.Coordinates.Z * 32);
				this.Buffer.WriteByte(this.Yaw);
				this.Buffer.WriteByte(this.Pitch);
				this.Buffer.WriteBool(this.OnGround);
				this.Buffer.FlushData();
			}
		}

		public static void Broadcast(Player player, Level level)
		{
			foreach (var i in level.OnlinePlayers)
			{
				if (i != player)
				{
					new EntityTeleport(i.Wrapper)
						{
							Coordinates = player.KnownPosition.ToVector3(), 
							OnGround = player.KnownPosition.OnGround, 
							UniqueServerID = player.EntityId, 
							Pitch = (byte)player.KnownPosition.Pitch, 
							Yaw = (byte)player.KnownPosition.Yaw
						}.Write();
				}
			}
		}
	}
}