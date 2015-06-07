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

namespace SharpMC.Utils
{
	using System;
	using System.ComponentModel;
	using System.IO;

	using SharpMC.Entity;
	using SharpMC.Enums;

	public class HealthManager
	{
		public HealthManager(Entity entity)
		{
			this.Entity = entity;
			this.ResetHealth();
			this.IsInvulnerable = false;
		}

		public Entity Entity { get; set; }

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
				writer.Write(this.Health);
				writer.Write(this.Air);
				writer.Write(this.FireTick);
				writer.Write(this.IsOnFire);
				buffer = stream.GetBuffer();
			}

			return buffer;
		}

		public void Import(byte[] data)
		{
			using (var stream = new MemoryStream(data))
			{
				var reader = new NbtBinaryReader(stream, false);
				this.Health = reader.ReadInt32();
				this.Air = reader.ReadInt16();
				this.FireTick = reader.ReadInt32();
				this.IsOnFire = reader.ReadBoolean();
			}
		}

		public void TakeHit(Player source, int damage = 1, DamageCause cause = DamageCause.Unknown)
		{
			if (this.LastDamageCause == DamageCause.Unknown)
			{
				this.LastDamageCause = cause;
			}

			this.LastDamageSource = source;

			this.Health -= damage;

			if (this.Entity == null)
			{
				return;
			}

			var player = this.Entity as Player;
			if (player != null)
			{
				player.SendHealth();
				player.BroadcastEntityAnimation(Animations.TakeDamage);
			}
		}

		public void Kill()
		{
			if (this.IsDead)
			{
				return;
			}

			this.IsDead = true;

			// Health = 0;
			// if (Player != null)
			// {
			// 	//player.SendSetHealth();
			// player.BroadcastEntityEvent();
			// player.BroadcastSetEntityData();
			// 	Player.SendHealth();
			// }

			// Entity.DespawnEntity();
		}

		public void ResetHealth()
		{
			this.Health = 20;
			this.Air = 300;
			this.Food = 20;
			this.FoodTick = 0;
			this.IsOnFire = false;
			this.FireTick = 0;
			this.IsDead = false;
			this.LastDamageCause = DamageCause.Unknown;
			this.RegenTick = 0;
		}

		public void OnTick()
		{
			var player = this.Entity as Player;
			if (this.IsDead)
			{
				return;
			}

			if (this.IsInvulnerable)
			{
				this.Health = 200;
			}

			if (this.Health <= 0)
			{
				this.IsDead = true;
				if (player != null)
				{
					this.Entity.Level.BroadcastChat(
						"§e"
						+ GetDescription(this.LastDamageCause)
							  .Replace("{0}", player.Username)
							  .Replace("{1}", this.LastDamageSource.Username));
				}

				return;
			}

			if (this.Food > 16 && this.Health < 20 && !this.IsOnFire)
			{
				// Regenerate health.
				if (this.RegenTick >= 80)
				{
					this.Health++;
					if (player != null)
					{
						player.SendHealth();
					}

					this.RegenTick = 0;
				}
				else
				{
					this.RegenTick++;
				}
			}

			if (this.FoodTick == 300)
			{
				if (this.Food > 0)
				{
					this.Food--;
				}
				else
				{
					this.Health--;
				}

				if (player != null)
				{
					player.SendHealth();
				}

				this.FoodTick = 0;
			}
			else
			{
				this.FoodTick += 1;
			}

			// TODO: Fix falldamage code below :P

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
			if (this.Health <= 0)
			{
				this.Kill();
				return;
			}

			if (player != null)
			{
				if (player.KnownPosition.Y < 0 && !this.IsDead)
				{
					this.TakeHit(player, 20, DamageCause.Void);
					return;
				}

				if (this.IsInWater(player.KnownPosition.ToVector3()))
				{
					this.Air--;
					if (this.Air <= 0)
					{
						if (Math.Abs(this.Air) % 10 == 0)
						{
							this.TakeHit(player, 1, DamageCause.Drowning);
						}
					}

					if (this.IsOnFire)
					{
						this.IsOnFire = false;
						this.FireTick = 0;
					}
				}
				else
				{
					this.Air = 300;
				}

				if (!this.IsOnFire && this.IsInLava(player.KnownPosition.ToVector3()))
				{
					this.FireTick = 300;
					this.IsOnFire = true;
				}

				if (this.IsOnFire)
				{
					this.FireTick--;
					if (this.FireTick <= 0)
					{
						this.IsOnFire = false;
					}

					if (Math.Abs(this.FireTick) % 20 == 0)
					{
						this.TakeHit(player, 1, DamageCause.Fire);
					}
				}
			}
		}

		private bool IsInWater(Vector3 playerPosition)
		{
			playerPosition.Y ++; // We want to check at 'head' height
			var block = this.Entity.Level.GetBlock(playerPosition);
			return block.Id == 8 || block.Id == 9;
		}

		private bool IsInLava(Vector3 playerPosition)
		{
			var block = this.Entity.Level.GetBlock(playerPosition);
			return block.Id == 10 || block.Id == 11;
		}

		public static string GetDescription(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
			var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}

			return value.ToString();
		}
	}
}