using System.Text;
using SharpMC.API.Entities;
using static SharpMC.Plugin.Pets.Commands.AbstractPetCommand;

namespace SharpMC.Plugin.Pets.Commands
{
    internal static class PetFindCommand
    {
        internal static bool Find(IPlayer player, int dist)
        {
            var world = player.World;
            var loc = player.Location;
            var res = FindNearby(world, loc, dist);
            var max = 9;
            var idx = 0;
            var bld = new StringBuilder();
            foreach (var entity in res)
            {
                if (!(entity is IMob animal))
                    continue;
                idx++;
                var al = animal.Location;
                var an = GetNameOf(animal);
                var adist = loc.Distance(al);
                var days = animal is IAgeable ageable ? ageable.Age : 0;
                var health = animal.Health;
                bld.Append($" #{idx} [{an}] {adist} m, {days} d, {health} pt %n");
                if (idx >= max)
                    break;
            }
            if (bld.Length >= 1)
            {
                bld.Insert(0, "Found following animals nearby:%n");
                player.SendChat(bld.ToString());
            }
            return true;
        }
    }
}