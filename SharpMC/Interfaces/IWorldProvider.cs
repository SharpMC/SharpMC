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
using System.Collections.Generic;
using SharpMC.Blocks;
using SharpMC.Entity;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Interfaces
{
	public class IWorldProvider
	{
		public virtual bool IsCaching { get; set; }

		public virtual void Initialize()
		{
		}

		public virtual ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			throw new NotImplementedException();
		}

		public virtual Vector3 GetSpawnPoint()
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, double playerX, double playerZ,
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, Player player, bool output = false)
		{
			throw new NotImplementedException();
		}

		public virtual void SaveChunks(string folder)
		{
			throw new NotImplementedException();
		}

		public virtual ChunkColumn LoadChunk(int x, int y)
		{
			throw new NotImplementedException();
		}

		public virtual void SetBlock(Block block, Level level, bool broadcast)
		{
			//throw new NotImplementedException();
		}

		public virtual void ClearCache()
		{
		}
	}
}