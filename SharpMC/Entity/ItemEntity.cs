using System.Timers;
using SharpMC.Enums;
using SharpMC.Networking.Packages;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Entity
{
	public class ItemEntity : Entity
	{
		public ItemStack Item { get; private set; }
		public int PickupDelay { get; set; }
		public int TimeToLive { get; set; }
		public ItemEntity(Level level, ItemStack item) : base(2, level)
		{
			Item = item;

			Height = 0.25;
			Width = 0.25;
			Length = 0.25;

			PickupDelay = 10;
			TimeToLive = 20*(5 * 60);
		}

		private void DespawnEntity(Player source)
		{
			TickTimer.Stop();

			foreach (Player i in Level.OnlinePlayers)
			{
				var SpawnedBy = i.Wrapper;
				if (source != null)
				{
					new CollectItem(SpawnedBy)
					{
						CollectorEntityId = source.EntityId,
						EntityId = EntityId
					}.Write();
				}

				new DestroyEntities(SpawnedBy)
				{
					EntityIds = new int[] {EntityId}
				}.Write();
			}
			Level.RemoveEntity(this);
		}

		public void SpawnEntity()
		{
			Level.AddEntity(this);
			foreach (Player i in Level.OnlinePlayers)
			{
				var SpawnedBy = i.Wrapper;
				new SpawnObject(SpawnedBy)
				{
					EntityId = EntityId,
					X = KnownPosition.X,
					Y = KnownPosition.Y,
					Z = KnownPosition.Z,
					Type = ObjectType.ItemStack,
					Data = Item
				}.Write();

				new EntityMetadata(SpawnedBy)
				{
					EntityId = EntityId,
					type = ObjectType.ItemStack,
					data = Item,
				}.Write();
			}
		}

		public override void OnTick()
		{
			TimeToLive--;
			PickupDelay--;

			if (TimeToLive <= 0)
			{
				DespawnEntity(null);
				return;
			}

			if (PickupDelay > 0) return;

			var players = Level.OnlinePlayers;
			foreach (var player in players)
			{
				if (KnownPosition.DistanceTo(player.KnownPosition) <= 1.8)
				{	
					player.Inventory.AddItem(Item.ItemId, Item.MetaData,1);

					DespawnEntity(player);
					break;
				}
			}
		}
	}
}
