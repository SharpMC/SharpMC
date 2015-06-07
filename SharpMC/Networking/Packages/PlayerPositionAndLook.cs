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
	using SharpMC.Utils;

	internal class PlayerPositionAndLook : Package<PlayerPositionAndLook>
	{
		public float Pitch = 0f;

		public double X = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().X;

		public double Y = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().Y;

		public float Yaw = 0f;

		public double Z = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().Z;

		public PlayerPositionAndLook(ClientWrapper client)
			: base(client)
		{
			this.SendId = 0x08;
			this.ReadId = 0x06;
		}

		public PlayerPositionAndLook(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			this.SendId = 0x08;
			this.ReadId = 0x06;
		}

		public override void Write()
		{
			if (this.Buffer != null)
			{
				this.Buffer.WriteVarInt(this.SendId);
				this.Buffer.WriteDouble(this.X);
				this.Buffer.WriteDouble(this.Y);
				this.Buffer.WriteDouble(this.Z);
				this.Buffer.WriteFloat(this.Yaw);
				this.Buffer.WriteFloat(this.Pitch);
				this.Buffer.WriteByte(111);
				this.Buffer.FlushData();
			}
		}

		public override void Read()
		{
			if (this.Buffer != null)
			{
				var X = this.Buffer.ReadDouble();
				var FeetY = this.Buffer.ReadDouble();
				var Z = this.Buffer.ReadDouble();
				var Yaw = this.Buffer.ReadFloat();
				var Pitch = this.Buffer.ReadFloat();
				var OnGround = this.Buffer.ReadBool();

				// Client.Player.KnownPosition.OnGround = OnGround;
				// Client.Player.KnownPosition.Yaw = Yaw;
				// Client.Player.KnownPosition.Pitch = Pitch;
				// Client.Player.KnownPosition = new PlayerLocation(X, FeetY, Z);

				// var movement = Client.Player.KnownPosition - originalCoordinates;
				// new EntityRelativeMove(Client) {Player = Client.Player, Movement = movement}.Broadcast(false, Client.Player);
				this.Client.Player.PositionChanged(new Vector3(X, FeetY, Z), Yaw, Pitch, OnGround);
			}
		}
	}
}