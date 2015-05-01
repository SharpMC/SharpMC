using System;
using System.ComponentModel;
using System.IO;
using SharpMC.Entity;
using SharpMC.Enums;

//using MiNET.Entities;

namespace SharpMC.Utils
{
	public class HealthManager
	{
		//public Entity LastDamageSource { get; set; }

		public HealthManager(Entity.Entity entity)
		{
			//Player = player;
			Entity = entity;
			ResetHealth();
		}

		public Entity.Entity Entity { get; set; }
		//public Player Player { get; set; }
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

			if (Health <= 0)
			{
				IsDead = true;
				if (player != null)
				{
					Entity.Level.BroadcastChat("§e" + GetDescription(LastDamageCause).Replace("{0}", player.Username));
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


				if (IsInWater(player.KnownPosition))
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

				if (!IsOnFire && IsInLava(player.KnownPosition))
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