using System.Collections.Generic;

namespace SharpMC.API.Entities
{
    public interface IEntityManager
    {
        int AddEntity(IEntity entity);

        void RemoveEntity(IEntity? caller, IEntity entity);

        IEnumerable<IEntity> Entities { get; }
    }
}