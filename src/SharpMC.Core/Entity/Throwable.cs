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
using System.Linq;
using SharpMC.Core.Blocks;
using SharpMC.Core.Enums;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Entity
{
	public class Throwable : Entity
	{
		protected Throwable(Player shooter, int entityTypeId, Level level) : base(entityTypeId, level)
		{
			Shooter = shooter;
			Ttl = 0;
			DespawnOnImpact = true;

			KnownPosition = shooter.KnownPosition;
		}

		public Player Shooter { get; set; }
		public int Ttl { get; set; }
		public bool DespawnOnImpact { get; set; }

		public override void OnTick()
		{
			base.OnTick();

			if (KnownPosition.Y <= 0
			    || (Velocity.Distance <= 0 && DespawnOnImpact)
			    || (Velocity.Distance <= 0 && !DespawnOnImpact && Ttl == 0))
			{
				DespawnEntity();
				return;
			}

			Ttl--;

			if (KnownPosition.Y <= 0 || Velocity.Distance <= 0) return;

			Velocity *= (1.0 - Drag);
			Velocity -= new Vector3(0, Gravity, 0);

			KnownPosition.X += (float) Velocity.X;
			KnownPosition.Y += (float) Velocity.Y;
			KnownPosition.Z += (float) Velocity.Z;

			var k = Math.Sqrt((Velocity.X*Velocity.X) + (Velocity.Z*Velocity.Z));
			KnownPosition.Yaw = (float) (Math.Atan2(Velocity.X, Velocity.Z)*180f/Math.PI);
			KnownPosition.Pitch = (float) (Math.Atan2(Velocity.Y, k)*180f/Math.PI);

			var bbox = GetBoundingBox();

			var playerHitted = CheckEntityCollide(bbox);

			bool collided;
			if (playerHitted != null)
			{
				playerHitted.HealthManager.TakeHit(playerHitted, 1, DamageCause.Projectile);
				collided = true;
			}
			else
			{
				collided = CheckBlockCollide(KnownPosition);
			}

			if (collided)
			{
				Velocity = Vector3.Zero;
			}

			BroadcastMoveAndMotion();
		}

		private Player CheckEntityCollide(BoundingBox bbox)
		{
			var players = Level.GetOnlinePlayers;
			foreach (var player in players)
			{
				if (player == Shooter) continue;

				if ((player.GetBoundingBox() + 0.3).Intersects(bbox))
				{
					return player;
				}
			}

			return null;
		}

		private bool CheckBlockCollide(PlayerLocation location)
		{
			var bbox = GetBoundingBox();
			var pos = location.ToVector3();

			var coords = new Vector3(
				(int) Math.Floor(KnownPosition.X),
				(int) Math.Floor((bbox.Max.Y + bbox.Min.Y)/2.0),
				(int) Math.Floor(KnownPosition.Z));

			var blocks = new Dictionary<double, Block>();

			for (var x = -1; x < 2; x++)
			{
				for (var z = -1; z < 2; z++)
				{
					for (var y = -1; y < 2; y++)
					{
						var block = Level.GetBlock(new Vector3(coords.X + x, coords.Y + y, coords.Z + z));
						if (block is BlockAir) continue;

						var blockbox = block.GetBoundingBox() + 0.3;
						if (blockbox.Intersects(GetBoundingBox()))
						{
							//if (!blockbox.Contains(KnownPosition.ToVector3())) continue;

							if (block is BlockFlowingLava || block is BlockStationaryLava)
							{
								//HealthManager.Ignite(1200);
								//Unimplemented should be done tho...
								continue;
							}

							if (!block.IsSolid) continue;

							blockbox = block.GetBoundingBox();

							var midPoint = blockbox.Min + 0.5;
							blocks.Add((pos - Velocity).DistanceTo(midPoint), block);
						}
					}
				}
			}

			if (blocks.Count == 0) return false;

			var firstBlock = blocks.OrderBy(pair => pair.Key).First().Value;

			var boundingBox = firstBlock.GetBoundingBox();
			if (!SetIntersectLocation(boundingBox, KnownPosition))
			{
				// No real hit
				return false;
			}

			// Use to debug hits, makes visual impressions (can be used for paintball too)
			var substBlock = new BlockStone {Coordinates = firstBlock.Coordinates};
			Level.SetBlock(substBlock);
			// End debug block

			Velocity = Vector3.Zero;
			return true;
		}

		public bool SetIntersectLocation(BoundingBox bbox, PlayerLocation location)
		{
			var ray = new Ray(location.ToVector3() - Velocity, Velocity.Normalize());
			var distance = ray.Intersects(bbox);
			if (distance != null)
			{
				var dist = (double) distance - 0.1;
				var pos = ray.Position + (ray.Direction*dist);
				KnownPosition.X = (float) pos.X;
				KnownPosition.Y = (float) pos.Y;
				KnownPosition.Z = (float) pos.Z;
				return true;
			}

			return false;
		}

		private void BroadcastMoveAndMotion()
		{
			foreach (var i in Level.OnlinePlayers)
			{
				new EntityTeleport(i.Wrapper)
				{
					UniqueServerID = EntityId,
					Coordinates = KnownPosition.ToVector3(),
					Yaw = (byte) KnownPosition.Yaw,
					Pitch = (byte) KnownPosition.Pitch,
					OnGround = true
				}.Write();
			}
		}
	}
}