using Microsoft.Extensions.Logging;
using SharpMC.API.Attributes;
using SharpMC.API.Entities;
using SharpMC.API.Plugins;
using SharpMC.Plugin.Pets.Commands;

namespace SharpMC.Plugin.Pets
{
    [Plugin]
    public class PetPlugin : IPlugin
    {
        private IPluginContext _context;

        public void OnEnable(IPluginContext context)
        {
            var log = context.GetLogger(this);

            log.LogInformation("Starting up...");
            _context = context;
            log.LogInformation("Startup done!");
        }

        [Command(Command = "mobcalm")]
        public void MobCalm(IPlayer player, int dist) => MobCalmCommand.Calm(player, dist);

        [Command(Command = "petfind")]
        public void PetFind(IPlayer player, int dist) => PetFindCommand.Find(player, dist);

        [Command(Command = "petheal")]
        public void PetHeal(IPlayer player, int dist) => PetHealCommand.Heal(player, dist);

        [Command(Command = "petlove")]
        public void PetLove(IPlayer player, int dist) => PetLoveCommand.Love(player, dist);

        [Command(Command = "petname")]
        public void PetName(IPlayer player, string name) => PetNameCommand.Tame(player, name);

        public void OnDisable()
        {
            var log = _context.GetLogger(this);

            log.LogInformation("Shutting down...");
            _context = null;
            log.LogInformation("Shutdown done!");
        }
    }
}