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

namespace SharpMC.Entity
{
	using SharpMC.Enums;
	using SharpMC.Networking.Packages;
	using SharpMC.Utils;
	using SharpMC.Worlds;

	public class ItemEntity : Entity
	{
		public ItemEntity(Level level, ItemStack item)
			: base(2, level)
		{
			this.Item = item;

			this.Height = 0.25;
			this.Width = 0.25;
			this.Length = 0.25;

			this.PickupDelay = 10;
			this.TimeToLive = 20 * (5 * 60);
		}

		public ItemStack Item { get; private set; }

		public int PickupDelay { get; set; }

		public int TimeToLive { get; set; }

		private void DespawnEntity(Player source)
		{
			this.TickTimer.Stop();

			foreach (var i in this.Level.OnlinePlayers)
			{
				var SpawnedBy = i.Wrapper;
				if (source != null)
				{
					new CollectItem(SpawnedBy) { CollectorEntityId = source.EntityId, EntityId = this.EntityId }.Write();
				}

				new DestroyEntities(SpawnedBy) { EntityIds = new[] { this.EntityId } }.Write();
			}

			this.Level.RemoveEntity(this);
		}

		public void SpawnEntity()
		{
			this.Level.AddEntity(this);
			foreach (var i in this.Level.OnlinePlayers)
			{
				var SpawnedBy = i.Wrapper;
				new SpawnObject(SpawnedBy)
					{
						EntityId = this.EntityId, 
						X = this.KnownPosition.X, 
						Y = this.KnownPosition.Y, 
						Z = this.KnownPosition.Z, 
						Type = ObjectType.ItemStack, 
						Data = this.Item
					}.Write();

				new EntityMetadata(SpawnedBy) { EntityId = this.EntityId, type = ObjectType.ItemStack, data = this.Item }.Write();
			}
		}

		public override void OnTick()
		{
			this.TimeToLive--;

			// 	PickupDelay--;
			if (this.TimeToLive <= 0)
			{
				this.DespawnEntity(null);
				return;
			}

			// 	if (PickupDelay > 0) return;
			var players = this.Level.OnlinePlayers;
			foreach (var player in players)
			{
				if (this.KnownPosition.DistanceTo(player.KnownPosition) <= 1.8)
				{
					player.Inventory.AddItem(this.Item.ItemId, this.Item.MetaData, 1);

					this.DespawnEntity(player);
					break;
				}

				new EntityTeleport(player.Wrapper)
					{
						UniqueServerID = this.EntityId, 
						Coordinates = this.KnownPosition.ToVector3(), 
						Yaw = (byte)this.KnownPosition.Yaw, 
						Pitch = (byte)this.KnownPosition.Pitch, 
						OnGround = true
					}.Write();
			}
		}
	}
}