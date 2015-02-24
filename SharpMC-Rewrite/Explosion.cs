using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Classes;

namespace SharpMCRewrite
{
	public class Explosion
	{
		private const int Ray = 16;
		private readonly IDictionary<IntVector3, Block> _afectedBlocks = new Dictionary<IntVector3, Block>();
		private readonly IntVector3 _centerCoordinates;
		private readonly float _size;
		private readonly Level _world;
		private readonly bool CoordsSet;
		private readonly bool Fire;

		/// <summary>
		///     Use this for Explosion an explosion only!
		/// </summary>
		/// <param name="world"></param>
		/// <param name="centerCoordinates"></param>
		/// <param name="size"></param>
		/// <param name="fire"></param>
		public Explosion(Level world, IntVector3 centerCoordinates, float size, bool fire = false)
		{
			_size = size;
			_centerCoordinates = centerCoordinates;
			_centerCoordinates.ConvertToHost();

			_world = world;
			CoordsSet = true;
			Fire = fire;
		}

		/// <summary>
		///     Only use this for SpawnTNT!
		/// </summary>
		public Explosion()
		{
			CoordsSet = false;
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
			if (!CoordsSet) throw new Exception("Please intiate using Explosion(Level, coordinates, size)");
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

							double cX = _centerCoordinates.X;
							double cY = _centerCoordinates.Y;
							double cZ = _centerCoordinates.Z;

							for (var blastForce2 = 0.3F; blastForce1 > 0.0F; blastForce1 -= blastForce2*0.75F)
							{
								var bx = (int) Math.Floor(cX);
								var by = (int) Math.Floor(cY);
								var bz = (int) Math.Floor(cZ);
								var block = _world.GetBlock(new IntVector3(bx, by, bz));

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

			//_size *= 2.0F;
			return true;
		}

		private bool SecondaryExplosion()
		{
			//Vector3 source = new Vector3(_centerCoordinates.X, _centerCoordinates.Y, _centerCoordinates.Z).Floor();
			//var yield = (1/_size)*100;
			//var explosionSize = _size*2;
			//var minX = Math.Floor(_centerCoordinates.X - explosionSize - 1);
			//var maxX = Math.Floor(_centerCoordinates.X + explosionSize + 1);
			//var minY = Math.Floor(_centerCoordinates.Y - explosionSize - 1);
			//var maxY = Math.Floor(_centerCoordinates.Y + explosionSize + 1);
			//var minZ = Math.Floor(_centerCoordinates.Z - explosionSize - 1);
			//var maxZ = Math.Floor(_centerCoordinates.Z + explosionSize + 1);
			//var explosionBB = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

			var records = new Records();
			foreach (var block in _afectedBlocks.Values)
			{
				records.Add(block.Coordinates - _centerCoordinates);
			}

			new Task(() =>
			{
				/*var mcpeExplode = McpeExplode.CreateObject();
				/mcpeExplode.x = _centerCoordinates.X;
				mcpeExplode.y = _centerCoordinates.Y;
				mcpeExplode.z = _centerCoordinates.Z;
				mcpeExplode.radius = _size;
				mcpeExplode.records = records;
				_world.RelayBroadcast(mcpeExplode);*/
			}).Start();

			foreach (var block in _afectedBlocks.Values)
			{
				//block.BreakBlock(Globals.Level);
				new Task(() => _world.SetBlock(new BlockAir {Coordinates = block.Coordinates})).Start();
				//new Task(() => block1.BreakBlock(_world)).Start();
				//if (block.Id != 0) new Task(() => Networking.Packages.ChunkData.Broadcast(_world.Generator.GetChunk(_centerCoordinates.X / 16, _centerCoordinates.Z / 16))).Start();
				if (block is BlockTNT)
				{
					new Task(() => SpawnTNT(block.Coordinates, _world)).Start();
				}
			}

			// Set stuff on fire
			if (Fire)
			{
				var random = new Random();
				foreach (var coord in _afectedBlocks.Keys)
				{
					var block = _world.GetBlock(coord);
					if (block is BlockAir)
					{
						var blockDown = _world.GetBlock(new IntVector3(coord.X, coord.Y--, coord.Z));
						if (!(blockDown is BlockAir) && random.Next(3) == 0)
						{
							_world.SetBlock(new BlockFire {Coordinates = block.Coordinates});
						}
					}
				}
			}

			return true;
		}

		private void SpawnTNT(IntVector3 blockCoordinates, Level world)
		{
			var rand = new Random();
			/*new PrimedTnt(world)
			{
				KnownPosition = new PlayerLocation
				{
					X = blockCoordinates.X,
					Y = blockCoordinates.Y,
					Z = blockCoordinates.Z,
				},
				Fuse = (byte)(rand.Next(0, 20) + 10)
			}.SpawnEntity();*/
		}
	}
}