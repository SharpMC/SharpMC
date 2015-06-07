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

namespace SharpMC.Worlds.Standard.Structures
{
	using System;

	using SharpMC.Blocks;
	using SharpMC.Utils;

	internal class PineTree : Structure
	{
		private readonly int LeafRadius = 2;

		private int BottomSpace = 2;

		public override string Name
		{
			get
			{
				return "PineTree";
			}
		}

		public override int Height
		{
			get
			{
				return 10;
			}
		}

		public bool ValidLocation(Vector3 location)
		{
			if (location.X - this.LeafRadius < 0 || location.X + this.LeafRadius >= 16 || location.Z - this.LeafRadius < 0
			    || location.Z + this.LeafRadius >= 256)
			{
				return false;
			}

			return true;
		}

		public override void Create(ChunkColumn chunk, int x, int y, int z)
		{
			var location = new Vector3(x, y, z);
			if (!this.ValidLocation(new Vector3(x, y, z)))
			{
				return;
			}

			var R = new Random();
			var Height = R.Next(7, 8);
			GenerateColumn(chunk, location, Height, new Block(17) { Metadata = 1 });
			for (var Y = 1; Y < Height; Y++)
			{
				if (Y % 2 == 0)
				{
					this.GenerateVanillaCircle(
						chunk, 
						location + new Vector3(0, Y + 1, 0), 
						this.LeafRadius - 1, 
						new Block(18) { Metadata = 1 });
					continue;
				}

				this.GenerateVanillaCircle(
					chunk, 
					location + new Vector3(0, Y + 1, 0), 
					this.LeafRadius, 
					new Block(18) { Metadata = 1 });
			}

			this.GenerateTopper(chunk, location + new Vector3(0, Height, 0), 0x1);
		}

		protected void GenerateTopper(ChunkColumn chunk, Vector3 location, byte type = 0x0)
		{
			var SectionRadius = 1;
			this.GenerateCircle(chunk, location, SectionRadius, new Block(18) { Metadata = 1 });
			var top = location + new Vector3(0, 1, 0);
			var x = (int)location.X;
			var y = (int)location.Y + 1;
			var z = (int)location.Z;
			chunk.SetBlock(x, y, z, new Block(18) { Metadata = 1 });
			if (type == 0x1 && y < 256)
			{
				this.GenerateVanillaCircle(chunk, new Vector3(x, y, z), SectionRadius, new Block(18) { Metadata = 1 });
			}
		}
	}
}