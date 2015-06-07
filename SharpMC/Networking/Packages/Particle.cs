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

	public class Particle : Package<Particle>
	{
		public int[] Data;

		public bool LongDistance = false;

		public float OffsetX = 0f;

		public float OffsetY = 0f;

		public float OffsetZ = 0f;

		public int ParticleCount = 1;

		public float ParticleData = 0f;

		public int ParticleId = 0;

		public float X = 0f;

		public float Y = 0f;

		public float Z = 0f;

		public Particle(ClientWrapper client)
			: base(client)
		{
			this.SendId = 0x2A;
		}

		public Particle(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			this.SendId = 0x2A;
		}

		public override void Write()
		{
			if (this.Buffer != null)
			{
				this.Buffer.WriteVarInt(this.SendId);
				this.Buffer.WriteInt(this.ParticleId);
				this.Buffer.WriteBool(this.LongDistance);
				this.Buffer.WriteFloat(this.X);
				this.Buffer.WriteFloat(this.Y);
				this.Buffer.WriteFloat(this.Z);
				this.Buffer.WriteFloat(this.OffsetX);
				this.Buffer.WriteFloat(this.OffsetY);
				this.Buffer.WriteFloat(this.OffsetZ);
				this.Buffer.WriteFloat(this.ParticleData);
				this.Buffer.WriteInt(this.ParticleCount);
				foreach (var i in this.Data)
				{
					this.Buffer.WriteVarInt(i);
				}

				this.Buffer.FlushData();
			}
		}
	}
}