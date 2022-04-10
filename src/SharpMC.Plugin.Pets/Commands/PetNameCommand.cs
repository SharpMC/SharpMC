using SharpMC.API.Entities;
using static SharpMC.Plugin.Pets.Commands.AbstractPetCommand;

namespace SharpMC.Plugin.Pets.Commands
{
    internal static class PetNameCommand
    {
        internal static bool Tame(IPlayer player, string newName)
        {
            var world = player.World;
            var loc = player.Location;
            var dist = 30;
            var res = FindNearby(world, loc, dist);
            foreach (var entity in res)
            {
                if (!(entity is ITameable animal))
                    continue;
                var origName = GetNameOf(animal);
                var name = string.IsNullOrWhiteSpace(newName)
                    ? $"T{animal.UniqueId:N}"
                    : newName;
                animal.CustomName = name;
                if (animal.IsTamed)
                {
                    player.SendChat($"Renamed '{origName}' to '{name}'!");
                    continue;
                }
                animal.Owner = player;
                player.SendChat($"Tamed {origName} as '{name}' for you!");
                break;
            }
            return true;
        }
    }
}