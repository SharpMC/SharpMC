using System;
using System.Linq;
using SharpMC.API.Attributes;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.API.Plugins;

namespace SharpMC.Plugin.Admin
{
    [Plugin]
    public class MainPlugin : IPlugin
    {
        private IPluginContext _context;

        public void OnEnable(IPluginContext context)
        {
            _context = context;
        }

        public void OnDisable()
        {
        }

        [Permission(Permission = "Core.World")]
        [Command(Command = "world")]
        public void WorldCommand(IPlayer player, string world)
        {
            switch (world)
            {
                case "overworld":
                    player.SendChat("Teleporting you to the Overworld!");
                    _context.LevelManager.TeleportToMain(player);
                    break;
                case "nether":
                    player.SendChat("Teleporting you to the Nether!");
                    _context.LevelManager.TeleportToLevel(player, "nether");
                    break;
                default:
                    player.SendChat("Unknown world! Choices: overworld, nether");
                    break;
            }
        }

        [Permission(Permission = "Core.TNT")]
        [Command(Command = "tnt")]
        public void TntCommand(IPlayer player)
        {
            var rand = new Random();
            // TODO
            /*
                new ActivatedTntEntity(player.Level)
                {
                    KnownPosition = player.KnownPosition, Fuse = rand.Next(0, 20) + 10
                }
                .SpawnEntity(); 
            */
        }

        [Permission(Permission = "Core.Tps")]
        [Command(Command = "TPS")]
        public void TpsCommand(IPlayer player)
        {
            _context.LevelManager.MainLevel.CalculateTps(player);
        }

        [Permission(Permission = "Core.Save")]
        [Command(Command = "save-all")]
        public void SaveAllCommand(IPlayer player)
        {
            foreach (var lvl in _context.LevelManager.GetLevels())
            {
                lvl.SaveChunks();
            }

            _context.LevelManager.MainLevel.SaveChunks();
        }

        [Permission(Permission = "Core.Gamemode")]
        [Command(Command = "gamemode")]
        public void Gamemode(IPlayer player, int gamemode)
        {
            switch (gamemode)
            {
                case 0:
                    player.Gamemode = GameMode.Survival;
                    break;
                case 1:
                    player.Gamemode = GameMode.Creative;
                    break;
                case 2:
                    player.Gamemode = GameMode.Adventure;
                    break;
                case 3:
                    player.Gamemode = GameMode.Spectator;
                    break;
            }
        }

        [Permission(Permission = "Core.StopServer")]
        [Command(Command = "stopserver")]
        public void StopServer(IPlayer player, string message)
        {
            var globals = _context.Globals;
            globals.StopServer(message);
        }

        [Permission(Permission = "Core.Time")]
        [Command(Command = "time")]
        public void Time(IPlayer player)
        {
            player.SendChat(player.Level.WorldTime.ToString());
        }

        [Permission(Permission = "Core.Time")]
        [Command(Command = "settime")]
        public void SetTime(IPlayer player, int time)
        {
            player.Level.WorldTime = time;
            player.SendChat("Time set to: " + time);
        }

        [Permission(Permission = "Core.Rain")]
        [Command(Command = "rain")]
        public void Rain(IPlayer player)
        {
            player.Level.Timetorain = 0;
        }

        [Permission(Permission = "Core.Rain")]
        [Command(Command = "raintime", Description = "Set time until next rain or length of current rain")]
        public void Rain(IPlayer player, int time)
        {
            player.Level.Timetorain = time;
        }

        [Permission(Permission = "Core.Msg")]
        [Command(Command = "msg")]
        public void Msg(IPlayer player, IPlayer target, string message)
        {
            target.SendChat(player.Username + ": " + message);
            player.SendChat("Message sent to: " + target.Username);
        }

        [Permission(Permission = "Core.TP")]
        [Command(Command = "tp")]
        public void Tp(IPlayer player, IPlayer target, IPlayer target2 = null)
        {
            if (target2 != null)
            {
                target.PositionChanged(target2.KnownPosition.ToVector3(), target2.KnownPosition.Yaw);
                player.SendChat("Teleported " + target.Username + "to: " + target2.Username);
                target.SendChat("You've been teleported to: " + target2.Username);
            }
            else
            {
                player.PositionChanged(target.KnownPosition.ToVector3(), target.KnownPosition.Yaw);
                player.SendChat("Teleported you to: " + target.Username);
            }
        }

        [Permission(Permission = "Core.Me")]
        [Command(Command = "me")]
        public void Me(IPlayer player, string[] message)
        {
            var fullMsg = "§5* §6" + player.Username + "§5 " +
                          message.Aggregate("", (current, i) => current + (i + " "));
            var globals = _context.Globals;
            globals.BroadcastChat(fullMsg, player);
            player.SendChat(fullMsg);
        }

        [Permission(Permission = "Core.Broadcast")]
        [Command(Command = "broadcast")]
        public void Broadcast(IPlayer player, string[] message)
        {
            var fullMsg = $"<{player.Username}> {message.Aggregate("", (current, i) => current + (i + " "))}";
            var globals = _context.Globals;
            globals.BroadcastChat(fullMsg, player);
            player.SendChat(fullMsg);
        }

        [Permission(Permission = "Core.Kick")]
        [Command(Command = "kick")]
        public void Kick(IPlayer player, IPlayer target, string message = "You got da boot!")
        {
            target.Kick(message);
        }

        [Permission(Permission = "Core.Op")]
        [Command(Command = "op")]
        public void Op(IPlayer player, IPlayer target)
        {
            if (target.ToggleOperatorStatus())
            {
                target.SendChat("You are now an Operator!", ChatColor.Yellow);
                player.SendChat($"Player \"{target.Username}\" is now an Operator!", ChatColor.Yellow);
            }
            else
            {
                target.SendChat("You have been De-Opped!", ChatColor.Yellow);
                player.SendChat($"Player \"{target.Username}\" has been De-Opped!", ChatColor.Yellow);
            }
        }
    }
}