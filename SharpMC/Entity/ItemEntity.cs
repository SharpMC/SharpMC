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
using SharpMC.Enums;
using SharpMC.Networking.Packages;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Entity
{
	public class ItemEntity : Entity
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

			foreach (var i in Level.OnlinePlayers)
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
					EntityIds = new[] {EntityId}
				}.Write();
			}
			Level.RemoveEntity(this);
		}

		public void SpawnEntity()
		{
			Level.AddEntity(this);
			foreach (var i in Level.OnlinePlayers)
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
					data = Item
				}.Write();
			}
		}

		public override void OnTick()
		{
			TimeToLive--;
			//	PickupDelay--;

			if (TimeToLive <= 0)
			{
				DespawnEntity(null);
				return;
			}

			//	if (PickupDelay > 0) return;

			var players = Level.OnlinePlayers;
			foreach (var player in players)
			{
				if (KnownPosition.DistanceTo(player.KnownPosition) <= 1.8)
				{
					player.Inventory.AddItem(Item.ItemId, Item.MetaData, 1);

					DespawnEntity(player);
					break;
				}
				new EntityTeleport(player.Wrapper)
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