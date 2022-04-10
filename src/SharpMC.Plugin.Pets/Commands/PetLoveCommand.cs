using SharpMC.API.Entities;
using static SharpMC.Plugin.Pets.Commands.AbstractPetCommand;

namespace SharpMC.Plugin.Pets.Commands
{
    internal static class PetLoveCommand
    {
        internal static bool Love(IPlayer player, int dist)
        {
            var world = player.World;
            var loc = player.Location;
            var res = FindNearby(world, loc, dist);
            var pets = 0;
            foreach (var entity in res)
            {
                if (!(entity is IAnimal animal))
                    continue;
                animal.Breed = true;
                const int ticks = 600;
                animal.LoveModeTicks = ticks;
                pets++;
            }
            player.SendChat($"Injected love into {pets} pet(s).");
            return true;
        }
    }
}