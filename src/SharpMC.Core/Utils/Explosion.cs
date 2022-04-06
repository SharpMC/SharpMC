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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpMC.Core.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Utils
{
	public class Explosion
	{
		private const int Ray = 16;
		private readonly IDictionary<Vector3, Block> _afectedBlocks = new Dictionary<Vector3, Block>();
		private readonly float _size;
		private readonly Level _world;
		private readonly bool _coordsSet;
		private readonly bool _fire;
		private Vector3 _centerCoordinates;

		/// <summary>
		///     Use this for Explosion an explosion only!
		/// </summary>
		/// <param name="world"></param>
		/// <param name="centerCoordinates"></param>
		/// <param name="size"></param>
		/// <param name="fire"></param>
		public Explosion(Level world, Vector3 centerCoordinates, float size, bool fire = false)
		{
			_size = size;
			_centerCoordinates = centerCoordinates;
			_world = world;
			_coordsSet = true;
			_fire = fire;
		}

		/// <summary>
		///     Only use this for SpawnTNT!
		/// </summary>
		public Explosion()
		{
			_coordsSet = false;
		}

		public bool Explode()
		{
			if (PrimaryExplosion())
			{
				return SecondaryExplosion();
			}

			return false;
		}

		private bool PrimaryExplosion()
		{
			if (!_coordsSet) throw new Exception("Please intiate using Explosion(Level, coordinates, size)");
			if (_size < 0.1) return false;

			for (var i = 0; i < Ray; i++)
			{
				for (var j = 0; j < Ray; j++)
				{
					for (var k = 0; k < Ray; k++)
					{
						if (i == 0 || i == Ray - 1 || j == 0 || j == Ray - 1 || k == 0 || k == Ray - 1)
						{
							double x = i/(Ray - 1.0F)*2.0F - 1.0F;
							double y = j/(Ray - 1.0F)*2.0F - 1.0F;
							double z = k/(Ray - 1.0F)*2.0F - 1.0F;
							var d6 = Math.Sqrt(x*x + y*y + z*z);

							x /= d6;
							y /= d6;
							z /= d6;
							var blastForce1 = (float) (_size*(0.7F + new Random().NextDouble()*0.6F));

							var cX = _centerCoordinates.X;
							var cY = _centerCoordinates.Y;
							var cZ = _centerCoordinates.Z;

							for (var blastForce2 = 0.3F; blastForce1 > 0.0F; blastForce1 -= blastForce2*0.75F)
							{
								var bx = (int) Math.Floor(cX);
								var by = (int) Math.Floor(cY);
								var bz = (int) Math.Floor(cZ);
								var block = _world.GetBlock(new Vector3(bx, by, bz));

								if (block.Id != 0)
								{
									var blastForce3 = block.GetHardness();
									blastForce1 -= (blastForce3 + 0.3F)*blastForce2;
								}

								if (blastForce1 > 0.0F)
								{
									if (!_afectedBlocks.ContainsKey(block.Coordinates) && block.Id != 0)
										_afectedBlocks.Add(block.Coordinates, block);
								}

								cX += x*blastForce2;
								cY += y*blastForce2;
								cZ += z*blastForce2;
							}
						}
					}
				}
			}
			return true;
		}

		private bool SecondaryExplosion()
		{
			var records = new Records();
			foreach (var block in _afectedBlocks.Values)
			{
				records.Add(block.Coordinates - _centerCoordinates);
			}


			foreach (var block in _afectedBlocks.Values)
			{
				var block1 = block;
				_world.SetBlock(new BlockAir {Coordinates = block1.Coordinates});

				if (block is BlockTnt)
				{
					new Task(() => SpawnTnt(block1.Coordinates, _world)).Start();
				}
			}

			// Set stuff on fire
			if (_fire)
			{
				var random = new Random();
				foreach (var coord in _afectedBlocks.Keys)
				{
					var block = _world.GetBlock(new Vector3(coord.X, coord.Y, coord.Z));
					if (block is BlockAir)
					{
						var blockDown = _world.GetBlock(new Vector3(coord.X, coord.Y - 1, coord.Z));
						if (!(blockDown is BlockAir) && random.Next(3) == 0)
						{
							_world.SetBlock(new BlockFire {Coordinates = block.Coordinates});
						}
					}
				}
			}

			return true;
		}

		private void SpawnTnt(Vector3 blockCoordinates, Level world)
		{
			var rand = new Random();
			new PrimedTNTEntity(world)
			{
				KnownPosition = blockCoordinates.ToPlayerLocation(),
				Fuse = (rand.Next(0, 20) + 10)
			}.SpawnEntity();
		}
	}
}