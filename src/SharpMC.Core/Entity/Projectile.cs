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
using System.Linq;
using SharpMC.Core.Blocks;
using SharpMC.Core.Enums;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Entity
{
	public class Projectile : Entity
	{
		protected Projectile(Player shooter, int entityTypeId, Level level) : base(entityTypeId, level)
		{
			Shooter = shooter;
			Ttl = 0;
			DespawnOnImpact = true;
			Collided = false;
			KnownPosition = new PlayerLocation(shooter.KnownPosition.X, shooter.KnownPosition.Y, shooter.KnownPosition.Z) {Yaw = shooter.KnownPosition.Yaw, Pitch = shooter.KnownPosition.Pitch};
		}

		public Player Shooter { get; set; }
		public int Ttl { get; set; }
		public bool DespawnOnImpact { get; set; }
		public bool Collided { get;set; }
		public ObjectType ObjectType { get; set; }

		public override void SpawnEntity()
		{
			Ttl = 9999999;
			Level.AddEntity(this);
			var addEntity = new SpawnObject(Shooter.Wrapper)
			{
				EntityId = EntityId,
				X = KnownPosition.X,
				Y = KnownPosition.Y,
				Z = KnownPosition.Z,
				Type = ObjectType,
				Info = Shooter.EntityId,
				Pitch = (byte) KnownPosition.Pitch,
				Yaw = (byte) KnownPosition.Yaw,
				VelocityX = (short)Velocity.X,
				VelocityY = (short)Velocity.Y,
				VelocityZ = (short)Velocity.Z,
			};
			Level.BroadcastPacket(addEntity);
		}

		public override void OnTick()
		{
			base.OnTick();

			if (Ttl <= 0 || KnownPosition.Y <= 0 || Collided)
			{
				DespawnEntity();
			}

			Ttl--;

			Entity entityCollided = CheckEntityCollide(KnownPosition.ToVector3(), Velocity);

			bool collided = false;
			if (entityCollided != null)
			{
				entityCollided.HealthManager.TakeHit(Shooter, 2, DamageCause.Projectile);
				collided = true;
			}
			else
			{
				//collided = CheckBlockCollide(KnownPosition);
				if (!collided)
				{
					var velocity2 = Velocity;
					velocity2 *= (1.0d - Drag);
					velocity2 -= new Vector3(0, Gravity, 0);
					double distance = velocity2.Distance;
					velocity2 = velocity2.Normalize() / 2;

					for (int i = 0; i < Math.Ceiling(distance) * 2; i++)
					{
						PlayerLocation nextPos = (PlayerLocation)KnownPosition.Clone();
						nextPos.X += (float)velocity2.X * i;
						nextPos.Y += (float)velocity2.Y * i;
						nextPos.Z += (float)velocity2.Z * i;

						Vector3 coord = new Vector3(nextPos.ToVector3());
						Block block = Level.GetBlock(coord);
						collided = block.Id != 0 && (block.GetBoundingBox()).Contains(nextPos.ToVector3());
						if (collided)
						{
							var substBlock = new Block(57) { Coordinates = block.Coordinates };
							Level.SetBlock(substBlock);
							KnownPosition = nextPos;
							SetIntersectLocation(block.GetBoundingBox(), KnownPosition);
							break;
						}
					}
				}
			}

			if (collided)
			{
				Collided = collided;
				Velocity = Vector3.Zero;
			}
			else
			{
				KnownPosition.X += (float)Velocity.X;
				KnownPosition.Y += (float)Velocity.Y;
				KnownPosition.Z += (float)Velocity.Z;

				Velocity *= (1.0 - Drag);
				Velocity -= new Vector3(0, Gravity, 0);

				var k = Math.Sqrt((Velocity.X * Velocity.X) + (Velocity.Z * Velocity.Z));
				KnownPosition.Yaw = (float)(Math.Atan2(Velocity.X, Velocity.Z) * 180f / Math.PI);
				KnownPosition.Pitch = (float)(Math.Atan2(Velocity.Y, k) * 180f / Math.PI);
			}

			BroadcastMoveAndMotion();
		}

		private Entity CheckEntityCollide(Vector3 position, Vector3 direction)
		{
			var players = Level.GetOnlinePlayers.OrderBy(player => position.DistanceTo(player.KnownPosition.ToVector3()));
			Ray2 ray = new Ray2
			{
				x = position,
				d = direction.Normalize()
			};

			foreach (var entity in players)
			{
				if (entity == Shooter) continue;

				if (Intersect(entity.GetBoundingBox(), ray))
				{
					if (ray.tNear < direction.Distance) break;

					Vector3 p = ray.x + ray.tNear * ray.d;
					KnownPosition = new PlayerLocation((float)p.X, (float)p.Y, (float)p.Z);
					return entity;
				}
			}

			var entities = Level.Entities.OrderBy(entity => position.DistanceTo(entity.KnownPosition.ToVector3()));
			foreach (var entity in entities)
			{
				if (entity == Shooter) continue;
				if (entity == this) continue;

				if (Intersect(entity.GetBoundingBox(), ray))
				{
					if (ray.tNear < direction.Distance) break;

					Vector3 p = ray.x + ray.tNear * ray.d;
					KnownPosition = new PlayerLocation((float)p.X, (float)p.Y, (float)p.Z);
					return entity;
				}
			}

			return null;
		}

		public static Vector3 GetMinimum(BoundingBox bbox)
		{
			return bbox.Min;
		}

		public static Vector3 GetMaximum(BoundingBox bbox)
		{
			return bbox.Max;
		}

		public static bool Intersect(BoundingBox aabb, Ray2 ray)
		{
			Vector3 min = GetMinimum(aabb), max = GetMaximum(aabb);
			double ix = ray.x.X;
			double iy = ray.x.Y;
			double iz = ray.x.Z;
			double t, u, v;
			bool hit = false;

			ray.tNear = Double.MaxValue;

			t = (min.X - ix) / ray.d.X;
			if (t < ray.tNear && t > -Ray2.EPSILON)
			{
				u = iz + ray.d.Z * t;
				v = iy + ray.d.Y * t;
				if (u >= min.Z && u <= max.Z &&
					v >= min.Y && v <= max.Y)
				{
					hit = true;
					ray.tNear = t;
					ray.u = u;
					ray.v = v;
					ray.n.X = -1;
					ray.n.Y = 0;
					ray.n.Z = 0;
				}
			}
			t = (max.X - ix) / ray.d.X;
			if (t < ray.tNear && t > -Ray2.EPSILON)
			{
				u = iz + ray.d.Z * t;
				v = iy + ray.d.Y * t;
				if (u >= min.Z && u <= max.Z &&
					v >= min.Y && v <= max.Y)
				{
					hit = true;
					ray.tNear = t;
					ray.u = 1 - u;
					ray.v = v;
					ray.n.X = 1;
					ray.n.Y = 0;
					ray.n.Z = 0;
				}
			}
			t = (min.Y - iy) / ray.d.Y;
			if (t < ray.tNear && t > -Ray2.EPSILON)
			{
				u = ix + ray.d.X * t;
				v = iz + ray.d.Z * t;
				if (u >= min.X && u <= max.X &&
					v >= min.Z && v <= max.Z)
				{
					hit = true;
					ray.tNear = t;
					ray.u = u;
					ray.v = v;
					ray.n.X = 0;
					ray.n.Y = -1;
					ray.n.Z = 0;
				}
			}
			t = (max.Y - iy) / ray.d.Y;
			if (t < ray.tNear && t > -Ray2.EPSILON)
			{
				u = ix + ray.d.X * t;
				v = iz + ray.d.Z * t;
				if (u >= min.X && u <= max.X &&
					v >= min.Z && v <= max.Z)
				{
					hit = true;
					ray.tNear = t;
					ray.u = u;
					ray.v = v;
					ray.n.X = 0;
					ray.n.Y = 1;
					ray.n.Z = 0;
				}
			}
			t = (min.Z - iz) / ray.d.Z;
			if (t < ray.tNear && t > -Ray2.EPSILON)
			{
				u = ix + ray.d.X * t;
				v = iy + ray.d.Y * t;
				if (u >= min.X && u <= max.X &&
					v >= min.Y && v <= max.Y)
				{
					hit = true;
					ray.tNear = t;
					ray.u = 1 - u;
					ray.v = v;
					ray.n.X = 0;
					ray.n.Y = 0;
					ray.n.Z = -1;
				}
			}
			t = (max.Z - iz) / ray.d.Z;
			if (t < ray.tNear && t > -Ray2.EPSILON)
			{
				u = ix + ray.d.X * t;
				v = iy + ray.d.Y * t;
				if (u >= min.X && u <= max.X &&
					v >= min.Y && v <= max.Y)
				{
					hit = true;
					ray.tNear = t;
					ray.u = u;
					ray.v = v;
					ray.n.X = 0;
					ray.n.Y = 0;
					ray.n.Z = 1;
				}
			}
			return hit;
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
			foreach (var i in Level.GetOnlinePlayers)
			{
				new EntityTeleport(i.Wrapper)
				{
					UniqueServerId = EntityId,
					Coordinates = KnownPosition.ToVector3(),
					Yaw = KnownPosition.Yaw,
					Pitch = KnownPosition.Pitch,
					OnGround = false
				}.Write();

				new EntityVelocity(i.Wrapper)
				{
					EntityId = EntityId,
					VelocityX = (short)Velocity.X,
					VelocityY = (short)Velocity.Y,
					VelocityZ = (short)Velocity.Z
				}.Write();
			}
		}
	}
}