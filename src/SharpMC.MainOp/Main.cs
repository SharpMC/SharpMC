#region Header

// Distributed under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015

#endregion

using SharpMC.Core;
using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.Plugins.API;

// ReSharper disable UnusedMember.Global

namespace SharpCore
{
    using System;
    using System.Linq;

    using SharpMC;

    public class Main : IPlugin
    {
        private PluginContext _context;

        public void OnEnable(PluginContext context)
        {
            this._context = context;
        }

        public void OnDisable()
        {
        }

        [Permission(Permission = "Core.World")]
        [Command(Command = "world")]
        public void WorldCommand(Player player, string world)
        {
            switch (world)
            {
                case "overworld":
                    player.SendChat("Teleporting you to the Overworld!");
                    this._context.LevelManager.TeleportToMain(player);
                    break;
                case "nether":
                    player.SendChat("Teleporting you to the Nether!");
                    this._context.LevelManager.TeleportToLevel(player, "nether");
                    break;
                default:
                    player.SendChat("Unknown world! Choices: overworld, nether");
                    break;
            }
        }

        [Permission(Permission = "Core.TNT")]
        [Command(Command = "tnt")]
        public void TntCommand(Player player)
        {
            var rand = new Random();
            new PrimedTNTEntity(player.Level) { KnownPosition = player.KnownPosition, Fuse = rand.Next(0, 20) + 10 }
                .SpawnEntity();
        }

        [Permission(Permission = "Core.Tps")]
        [Command(Command = "TPS")]
        public void TpsCommand(Player player)
        {
            this._context.LevelManager.MainLevel.CalculateTps(player);
        }

        [Permission(Permission = "Core.Save")]
        [Command(Command = "save-all")]
        public void SaveAllCommand(Player player)
        {
            foreach (var lvl in this._context.LevelManager.GetLevels())
            {
                lvl.SaveChunks();
            }

            this._context.LevelManager.MainLevel.SaveChunks();
        }

        [Permission(Permission = "Core.Gamemode")]
        [Command(Command = "gamemode")]
        public void Gamemode(Player player, int gamemode)
        {
            switch (gamemode)
            {
                case 0:
                    player.SetGamemode(SharpMC.Enums.Gamemode.Survival);
                    break;
                case 1:
                    player.SetGamemode(SharpMC.Enums.Gamemode.Creative);
                    break;
                case 2:
					player.SetGamemode(SharpMC.Enums.Gamemode.Adventure);
                    break;
                case 3:
					player.SetGamemode(SharpMC.Enums.Gamemode.Spectator);
                    break;
            }
        }

        [Permission(Permission = "Core.StopServer")]
        [Command(Command = "stopserver")]
        public void StopServer(Player player, string message)
        {
            Globals.StopServer(message);
        }

        [Permission(Permission = "Core.Time")]
        [Command(Command = "time")]
        public void Time(Player player)
        {
            player.SendChat(player.Level.GetWorldTime().ToString());
        }

        [Permission(Permission = "Core.Time")]
        [Command(Command = "settime")]
        public void SetTime(Player player, int time)
        {
            player.Level.SetWorldTime(time);
            player.SendChat("Time set to: " + time);
        }

        [Permission(Permission = "Core.Rain")]
        [Command(Command = "rain")]
        public void Rain(Player player)
        {
            player.Level.Timetorain = 0;
        }

        [Permission(Permission = "Core.Rain")]
        [Command(Command = "raintime", Description = "Set time until next rain or length of current rain")]
        public void Rain(Player player, int time)
        {
	        player.Level.Timetorain = time;
        }

        [Permission(Permission = "Core.Msg")]
        [Command(Command = "msg")]
        public void Msg(Player player, Player target, string message)
        {
            target.SendChat(player.Username + ": " + message);
            player.SendChat("Message sent to: " + target.Username);
        }

        [Permission(Permission = "Core.TP")]
        [Command(Command = "tp")]
        public void Tp(Player player, Player target, Player target2 = null)
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
        public void Me(Player player, string[] message)
        {
            string fullMsg = "§5* §6" + player.Username + "§5 " + message.Aggregate("", (current, i) => current + (i + " "));
            Globals.BroadcastChat(fullMsg, player);
            player.SendChat(fullMsg);
        }

        [Permission(Permission = "Core.Broadcast")]
        [Command(Command = "broadcast")]
        public void Broadcast(Player player, string[] message)
        {
            string fullMsg = "<" + player.Username + "> " + message.Aggregate("", (current, i) => current + (i + " "));
            Globals.BroadcastChat(fullMsg, player);
            player.SendChat(fullMsg);
        }

        [Permission(Permission = "Core.Kick")]
        [Command(Command = "kick")]
        public void Kick(Player player, Player target, string message = "You got da boot!")
        {
            var obj = new McChatMessage(message);
            target.Kick(obj);
        }

		[Permission(Permission = "Core.Op")]
		[Command(Command = "op")]
	    public void Op(Player player, Player target)
		{
			if (target.ToggleOperatorStatus())
			{
				target.SendChat("You are now an Operator!", ChatColor.Yellow);
				player.SendChat(String.Format("Player \"{0}\" is now an Operator!", target.Username), ChatColor.Yellow);
			}
			else
			{
				target.SendChat("You have been De-Opped!", ChatColor.Yellow);
				player.SendChat(String.Format("Player \"{0}\" has been De-Opped!", target.Username), ChatColor.Yellow);
			}
		}
    }
}