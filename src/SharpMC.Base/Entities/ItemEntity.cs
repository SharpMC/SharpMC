using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Core.Entity
{
	public class ItemEntity : Entities.Entity
	{
		public ItemEntity(Level level, ItemStack item) : base(2, level)
		{
			Item = item;

			Height = 0.25;
			Width = 0.25;
			Length = 0.25;

			PickupDelay = 10;
			TimeToLive = 20*(5*60);
		}

		public ItemStack Item { get; private set; }
		public int PickupDelay { get; set; }
		public int TimeToLive { get; set; }

		private void DespawnEntity(Player source)
		{
			TickTimer.Stop();

			foreach (var i in Level.GetOnlinePlayers)
			{
				var spawnedBy = i.Wrapper;
				if (source != null)
				{
					new CollectItem(spawnedBy)
					{
						CollectorEntityId = source.EntityId,
						EntityId = EntityId
					}.Write();
				}

				new DestroyEntities(spawnedBy)
				{
					EntityIds = new[] {EntityId}
				}.Write();
			}
			Level.RemoveEntity(this);
		}

		public override void SpawnEntity()
		{
			Level.AddEntity(this);
			foreach (var i in Level.GetOnlinePlayers)
			{
				var spawnedBy = i.Wrapper;
				new SpawnObject(spawnedBy)
				{
					EntityId = EntityId,
					X = KnownPosition.X,
					Y = KnownPosition.Y,
					Z = KnownPosition.Z,
					Type = ObjectType.ItemStack,
				}.Write();

				new EntityMetadata(spawnedBy)
				{
					EntityId = EntityId,
					Type = ObjectType.ItemStack,
					Data = Item
				}.Write();
			}
		}

		public override void OnTick()
		{
			TimeToLive--;

			if (TimeToLive <= 0)
			{
				DespawnEntity(null);
				return;
			}

			var players = Level.GetOnlinePlayers;
			foreach (var player in players)
			{
				if (KnownPosition.DistanceTo(player.KnownPosition) <= 1.8)
				{
					player.Inventory.AddItem(Item.ItemId, Item.MetaData);

					DespawnEntity(player);
					break;
				}
				new EntityTeleport(player.Wrapper)
				{
					UniqueServerId = EntityId,
					Coordinates = KnownPosition.ToVector3(),
					Yaw = (byte) KnownPosition.Yaw,
					Pitch = (byte) KnownPosition.Pitch,
					OnGround = true
				}.Write();
			}
		}
	}
}