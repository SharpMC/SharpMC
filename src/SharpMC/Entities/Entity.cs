using System;
using SharpMC.Players;
using SharpMC.World;

namespace SharpMC.Entities
{
    public abstract class Entity
    {
        public int EntityId { get; set; } = -1;

        public Level Level { get; set; }
        public bool IsSpawned { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public PlayerLocation KnownPosition { get; set; }

        protected Entity(Level level)
        {
            Level = level;
            KnownPosition = new PlayerLocation();
            EntityId = EntityManager.EntityIdUndefined;
        }

        public virtual void OnTick()
        {
        }

        public virtual void SpawnEntity()
        {
            Level.AddEntity(this);
            IsSpawned = true;
        }

        public virtual void SpawnToPlayers(Player[] players)
        {
        }

        public virtual void DespawnEntity()
        {
            Level.RemoveEntity(this);
        }

        public virtual void DespawnFromPlayers(Player[] players)
        {
        }
    }
}