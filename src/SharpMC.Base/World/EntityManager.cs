using System;
using System.Threading;
using SharpMC.Entities;

namespace SharpMC.World
{
	public class EntityManager
	{
		public const int EntityIdUndefined = -1;

		private int _entityId = 1;

		public int AddEntity(Entity entity)
		{
			if (entity.EntityId == EntityIdUndefined)
			{
				entity.EntityId = Interlocked.Increment(ref _entityId);
			}

			return entity.EntityId;
		}

		public void RemoveEntity(Entity caller, Entity entity)
		{
			if (entity == caller) throw new Exception("Tried to REMOVE entity for self");
			if (entity.EntityId != EntityIdUndefined) entity.EntityId = EntityIdUndefined;
		}
	}
}
