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
using System.ComponentModel;
using System.IO;
using SharpMC.Entity;
using SharpMC.Enums;

namespace SharpMC.Utils
{
	public class HealthManager
	{
		public HealthManager(Entity.Entity entity)
		{
			Entity = entity;
			ResetHealth();
			IsInvulnerable = false;
		}

		public Entity.Entity Entity { get; set; }
		public int Health { get; set; }
		public short Air { get; set; }
		public short Food { get; set; }
		public short FoodTick { get; set; }
		public bool IsDead { get; set; }
		public int FireTick { get; set; }
		public bool IsOnFire { get; set; }
		public DamageCause LastDamageCause { get; set; }
		public Player LastDamageSource { get; set; }
		private int FallDamage { get; set; }
		private int FallTick { get; set; }
		private int RegenTick { get; set; }
		public bool IsInvulnerable { get; set; }

		public byte[] Export()
		{
			byte[] buffer;
			using (var stream = new MemoryStream())
			{
				var writer = new NbtBinaryWriter(stream, false);
				writer.Write(Health);
				writer.Write(Air);
				writer.Write(FireTick);
				writer.Write(IsOnFire);
				buffer = stream.GetBuffer();
			}
			return buffer;
		}

		public void Import(byte[] data)
		{
			using (var stream = new MemoryStream(data))
			{
				var reader = new NbtBinaryReader(stream, false);
				Health = reader.ReadInt32();
				Air = reader.ReadInt16();
				FireTick = reader.ReadInt32();
				IsOnFire = reader.ReadBoolean();
			}
		}

		public void TakeHit(Player source, int damage = 1, DamageCause cause = DamageCause.Unknown)
		{
			if (LastDamageCause == DamageCause.Unknown) LastDamageCause = cause;

			LastDamageSource = source;

			Health -= damage;

			if (Entity == null) return;

			var player = Entity as Player;
			if (player != null)
			{
				player.SendHealth();
				player.BroadcastEntityAnimation(Animations.TakeDamage);
			}
		}

		public void Kill()
		{
			if (IsDead) return;

			IsDead = true;
			//Health = 0;
			//if (Player != null)
			//{
			//	//player.SendSetHealth();
				//player.BroadcastEntityEvent();
				//player.BroadcastSetEntityData();
			//	Player.SendHealth();
			//}

			//Entity.DespawnEntity();
		}

		public void ResetHealth()
		{
			Health = 20;
			Air = 300;
			Food = 20;
			FoodTick = 0;
			IsOnFire = false;
			FireTick = 0;
			IsDead = false;
			LastDamageCause = DamageCause.Unknown;
			RegenTick = 0;
		}

		public void OnTick()
		{
			var player = Entity as Player;
			if (IsDead) return;

			if (IsInvulnerable) Health = 200;

			if (Health <= 0)
			{
				IsDead = true;
				if (player != null)
				{
					Entity.Level.BroadcastChat("§e" + GetDescription(LastDamageCause).Replace("{0}", player.Username).Replace("{1}", LastDamageSource.Username));
				}
				return;
			}

			if (Food > 16 && Health < 20 && !IsOnFire)
			{
				//Regenerate health.
				if (RegenTick >= 80)
				{
					Health++;
					if (player != null)
					{
						player.SendHealth();
					}
					RegenTick = 0;
				}
				else RegenTick++;
			}

			if (FoodTick == 300)
			{
				if (Food > 0)
				{
					Food--;
				}
				else
				{
					Health--;
				}
				if (player != null)
				{
					player.SendHealth();
				}
				FoodTick = 0;
			}
			else FoodTick += 1;


			//TODO: Fix falldamage code below :P

			/*if (!Player.OnGround)
			{
				if (FallTick > 3)
				{
					FallDamage++;
					FallTick = 0;
				}
				FallTick++;
			}
			else
			{
				if (FallDamage > 0)
				{
					//Health -= FallDamage;
					if (FallDamage > 3)
					{
						TakeHit(Player, FallDamage - 3, DamageCause.Fall);
					}

					FallDamage = 0;
					FallTick = 0;
				}
				if (FallTick > 0) FallTick = 0;
			}*/

			if (Health <= 0)
			{
				Kill();
				return;
			}

			if (player != null)
			{
				if (player.KnownPosition.Y < 0 && !IsDead)
				{
					TakeHit(player, 20, DamageCause.Void);
					return;
				}


				if (IsInWater(player.KnownPosition.ToVector3()))
				{
					Air--;
					if (Air <= 0)
					{
						if (Math.Abs(Air)%10 == 0)
						{
							TakeHit(player, 1, DamageCause.Drowning);
						}
					}

					if (IsOnFire)
					{
						IsOnFire = false;
						FireTick = 0;
					}
				}
				else
				{
					Air = 300;
				}

				if (!IsOnFire && IsInLava(player.KnownPosition.ToVector3()))
				{
					FireTick = 300;
					IsOnFire = true;
				}

				if (IsOnFire)
				{
					FireTick--;
					if (FireTick <= 0)
					{
						IsOnFire = false;
					}

					if (Math.Abs(FireTick)%20 == 0)
					{
						TakeHit(player, 1, DamageCause.Fire);
					}
				}
			}
		}

		private bool IsInWater(Vector3 playerPosition)
		{
			playerPosition.Y ++; //We want to check at 'head' height
			var block = Entity.Level.GetBlock(playerPosition);
			return block.Id == 8 || block.Id == 9;
		}

		private bool IsInLava(Vector3 playerPosition)
		{
			var block = Entity.Level.GetBlock(playerPosition);
			return block.Id == 10 || block.Id == 11;
		}

		public static string GetDescription(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
			var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
			if (attributes.Length > 0)
				return attributes[0].Description;
			return value.ToString();
		}
	}
}