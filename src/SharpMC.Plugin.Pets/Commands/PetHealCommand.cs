using SharpMC.API.Custom;
using SharpMC.API.Entities;
using static SharpMC.Plugin.Pets.Commands.AbstractPetCommand;

namespace SharpMC.Plugin.Pets.Commands
{
    internal static class PetHealCommand
    {
        internal static bool Heal(IPlayer player, int dist)
        {
            var world = player.World;
            var loc = player.Location;
            var res = FindNearby(world, loc, dist);
            var pets = 0;
            foreach (var entity in res)
            {
                if (!(entity is IAnimal a))
                    continue;
                var mh = a.GetAttribute(AttributeNames.GenericMaxHealth);
                var maxHealth = mh.Value;
                a.Health = maxHealth;
                pets++;
            }
            player.SendChat($"Healed up {pets} pet(s).");
            return true;
        }
    }
}