using System.Collections.Generic;
using System.Linq;
using SharpMC.API.Entities;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.Plugin.Pets.Commands
{
    internal static class AbstractPetCommand
    {
        internal static List<IEntity> FindNearby(IWorld world, ILocation loc, int dist)
        {
            var bx = dist;
            var by = dist;
            var bz = dist;
            return world.GetNearbyEntities(loc, bx, by, bz,
                    e => e is IMob)
                .OrderBy(e => loc.Distance(e.Location))
                .ToList();
        }

        internal static string GetNameOf(IMob animal)
        {
            var an = animal.CustomName;
            if (an == null || string.IsNullOrWhiteSpace(an))
            {
                return animal.Name;
            }
            return an;
        }
    }
}