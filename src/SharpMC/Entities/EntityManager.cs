using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using SharpMC.API.Entities;

namespace SharpMC.Entities
{
    internal sealed class EntityManager : IEntityManager, IDisposable
    {
        internal const int EntityIdUndefined = -1;
        private int _entityId;

        private readonly ConcurrentDictionary<int, IEntity> _entities;

        public EntityManager()
        {
            _entityId = 1;
            _entities = new ConcurrentDictionary<int, IEntity>();
        }

        public int AddEntity(IEntity entity)
        {
            if (entity.EntityId == EntityIdUndefined)
            {
                entity.EntityId = Interlocked.Increment(ref _entityId);
            }
            var res = entity.EntityId;
            _entities.TryAdd(res, entity);
            return res;
        }

        public void RemoveEntity(IEntity? caller, IEntity entity)
        {
            if (entity == caller)
                throw new Exception("Tried to remove entity for itself!");
            var currentId = entity.EntityId;
            _entities.TryRemove(currentId, out _);
            if (currentId != EntityIdUndefined)
                entity.EntityId = EntityIdUndefined;
        }

        public IEnumerable<IEntity> Entities => _entities.Values;

        public void Dispose()
        {
            _entities.Clear();
        }
    }
}