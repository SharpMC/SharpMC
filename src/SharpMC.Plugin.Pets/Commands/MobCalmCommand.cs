using SharpMC.API.Entities;

namespace SharpMC.Plugin.Pets.Commands
{
    internal static class MobCalmCommand
    {
        internal static bool Calm(IPlayer player, int dist)
        {
            var world = player.World;
            var loc = player.Location;
            var res = AbstractPetCommand.FindNearby(world, loc, dist);
            var monsters = 0;
            foreach (var entity in res)
            {
                if (!(entity is IMonster monster))
                    continue;
                monster.Aware = false;
                var amount = monster.Health - 1;
                monster.Damage(amount);
                monsters++;
            }
            player.SendChat($"Calmed down {monsters} monster(s).");
            return true;
        }
    }
}