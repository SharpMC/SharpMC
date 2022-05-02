using System;
using SharpMC.API.Entities;
using SharpMC.API.Players;
using SharpMC.API.Worlds;

namespace SharpMC.Entities
{
    public abstract class Entity : IEntity
    {
        public int EntityId { get; set; }

        public ILevel Level { get; set; }
        public bool IsSpawned { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public PlayerLocation KnownPosition { get; set; }

        protected Entity(ILevel level)
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

        public virtual void SpawnToPlayers(IPlayer[] players)
        {
        }

        public virtual void DespawnEntity()
        {
            Level.RemoveEntity(this);
        }

        public virtual void DespawnFromPlayers(IPlayer[] players)
        {
        }
    }
}