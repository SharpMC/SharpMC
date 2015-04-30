using System;
using System.ComponentModel;
using System.IO;
using SharpMC.Enums;
using SharpMC.Utils;

//using MiNET.Entities;

namespace SharpMC.Classes
{
	public class HealthManager
	{
		//public Entity LastDamageSource { get; set; }

		public HealthManager(Player player)
		{
			Player = player;
			ResetHealth();
		}

		//public Entity Entity { get; set; }
		public Player Player { get; set; }
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

			//Untested code below, should work fine, however this is not sure yet.
			//	int Damage = ItemFactory.GetItem(sourcePlayer.Inventory.ItemInHand.Value.Id).GetDamage();
			//	Health -= Damage - Entity.Armour;

			Health -= damage;

			//if (Player != null)
			//Player.SendSetHealth();
			Player.SendHealth();
			Player.BroadcastEntityAnimation(Animations.TakeDamage);
		}

		public void Kill()
		{
			if (IsDead) return;

			IsDead = true;
			Health = 0;
			if (Player != null)
			{
				//player.SendSetHealth();
				//player.BroadcastEntityEvent();
				//player.BroadcastSetEntityData();
				Player.SendHealth();
			}

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
		}

		public void OnTick()
		{
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
				Player.SendHealth();
				FoodTick = 0;
			}
			else FoodTick += 1;

			//TODO: Rewrite to fit all entities

			/*if (!Player.OnGround)
			{
				if (FallTick > 9)
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
					Health -= FallDamage;
					FallDamage = 0;
					FallTick = 0;
					Player.SendHealth();
				}
			}*/

			if (IsDead) return;

			if (Health <= 0)
			{
				Kill();
				return;
			}

			if (Player.Coordinates.Y < 0 && !IsDead)
			{
				TakeHit(null, 10);
				LastDamageCause = DamageCause.Void;
				return;
			}

			if (IsInWater(Player.Coordinates))
			{
				Air--;
				if (Air <= 0)
				{
					if (Math.Abs(Air)%10 == 0)
					{
						Health--;
						//var player = Entity as Player;
						if (Player != null)
						{
							//	player.SendSetHealth();
							//	player.BroadcastEntityEvent();
							//	player.BroadcastSetEntityData();
							Player.SendHealth();
							LastDamageCause = DamageCause.Drowning;
						}
					}
				}

				if (IsOnFire)
				{
					IsOnFire = false;
					FireTick = 0;
					//if (Player != null)
					//player.BroadcastSetEntityData();
				}
			}
			else
			{
				Air = 300;
			}

			if (!IsOnFire && IsInLava(Player.Coordinates))
			{
				FireTick = 300;
				IsOnFire = true;
				//var player = Entity as Player;
				//	if (Player != null)
				//		player.BroadcastSetEntityData();
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
					Health--;
					if (Player != null)
					{
						//	player.SendSetHealth();
						//	player.BroadcastEntityEvent();
						//	player.BroadcastSetEntityData();
						Player.SendHealth();
						LastDamageCause = DamageCause.FireTick;
					}
				}
			}
		}

		private bool IsInWater(Vector3 playerPosition)
		{
			var y = playerPosition.Y + 1;

			var waterPos = new Vector3(Math.Floor(playerPosition.X), Math.Floor(y), Math.Floor(playerPosition.Z));

			var block = Player.CurrentLevel.GetBlock(waterPos);

			if (block.Id != 8 && block.Id != 9) return false;

			return y < Math.Floor(y) + 1 - ((1.0/9.0) - 0.1111111);
		}

		private bool IsInLava(Vector3 playerPosition)
		{
			var block = Player.CurrentLevel.GetBlock(playerPosition);

			if (block == null || (block.Id != 10 && block.Id != 11)) return false;

			return playerPosition.Y < Math.Floor(playerPosition.Y) + 1 - ((1/9) - 0.1111111);
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