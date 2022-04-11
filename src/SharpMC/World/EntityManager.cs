using System;
using System.Threading;
using SharpMC.API.Entities;

namespace SharpMC.World
{
    public class EntityManager
    {
        public const int EntityIdUndefined = -1;

        private int _entityId = 1;

        public int AddEntity(IEntity entity)
        {
            if (entity.EntityId == EntityIdUndefined)
            {
                entity.EntityId = Interlocked.Increment(ref _entityId);
            }
            return entity.EntityId;
        }

        public void RemoveEntity(IEntity caller, IEntity entity)
        {
            if (entity == caller)
                throw new Exception("Tried to REMOVE entity for self");
            if (entity.EntityId != EntityIdUndefined)
                entity.EntityId = EntityIdUndefined;
        }
    }
}