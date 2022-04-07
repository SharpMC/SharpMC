using System;
using System.Threading;
using SharpMC.Entities;

namespace SharpMC.World
{
    public class EntityManager
    {
        public const int EntityIdUndefined = -1;

        private int _entityId = 1;

        private int GetNextEntityId()
        {
            return Interlocked.Increment(ref _entityId);
        }

        public int AddEntity(Entity entity)
        {
            if (entity.EntityId == EntityIdUndefined)
            {
                entity.EntityId = GetNextEntityId();
            }

            return entity.EntityId;
        }

        public void RemoveEntity(Entity caller, Entity entity)
        {
            if (entity == caller) throw new Exception("Tried to REMOVE entity for self");
            if (entity.EntityId != EntityIdUndefined) entity.EntityId = EntityIdUndefined;
        }

        private static readonly EntityManager _instance = new EntityManager();

        public static int GetEntityId()
        {
            return _instance.GetNextEntityId();
        }
    }
}