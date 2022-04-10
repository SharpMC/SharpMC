using System;
using System.Collections.Generic;
using SharpMC.API.Entities;
using SharpMC.API.Utils;

namespace SharpMC.API.Worlds
{
    public interface IWorld
    {
        IEnumerable<IEntity> GetNearbyEntities(ILocation loc, int x, int y, int z, 
            Func<IEntity, bool> acceptor);
    }
}