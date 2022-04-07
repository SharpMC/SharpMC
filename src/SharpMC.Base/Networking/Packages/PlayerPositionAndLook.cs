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

using SharpMC.Core.Utils;
using SharpMC.Util;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerPositionAndLook : Package<PlayerPositionAndLook>
	{
		public double X = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().X;
		public double Y = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().Y;
		public double Z = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().Z;
		public float Yaw = 0f;
		public float Pitch = 0f;

		public PlayerPositionAndLook(ClientWrapper client) : base(client)
		{
			SendId = 0x08;
			ReadId = 0x06;
		}

		public PlayerPositionAndLook(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x08;
			ReadId = 0x06;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteDouble(X);
				Buffer.WriteDouble(Y);
				Buffer.WriteDouble(Z);
				Buffer.WriteFloat(Yaw);
				Buffer.WriteFloat(Pitch);
				Buffer.WriteByte(111);
				Buffer.FlushData();
			}
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var x = Buffer.ReadDouble();
				var feetY = Buffer.ReadDouble();
				var z = Buffer.ReadDouble();
				var yaw = Buffer.ReadFloat();
				var pitch = Buffer.ReadFloat();
				var onGround = Buffer.ReadBool();

				Client.Player.PositionChanged(
                    Vectors.Create(x, feetY, z), yaw, pitch, onGround);
			}
		}
	}
}