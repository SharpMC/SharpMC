# SharpMC

What is SharpMC?
----------------
SharpMC is a Minecraft server written in C# made by Kennyvv.
It is a hobby project, the goal is to create an alternative to Spigot/Bukkit/Sponge.
This fork is made by xafero, updating everything and generating network protocol.

Screenshots
----------------
![Screenshot1](/wiki/lobby1.png?raw=true)
![Screenshot2](/wiki/ingame1.png?raw=true)

Current Specs
-----------------
  - Protocol: #758 (1.18.2)
  - World Format: Custom
  - Multiworld: Supported
  - Physics: Partial
  - Entities: Partial
  - Scripting: C# Plugins
  - Platforms: Windows, Mac, Linux under .NET 6

How to run?
----------------
* git clone https://github.com/xafero/SharpMC.git
* cd src
* cd SharpMC.Server
* dotnet run

It should now listen on localhost (127.0.0.1).

What is SharpCore?
----------------
SharpCore is the main plugin for SharpMC which provides the basic most basic commands needed for server operation.

List of Commands & Permissions
------------------------------

| Command               | Permission                      | Description                                     |
|-----------------------|---------------------------------|-------------------------------------------------|
| /world                | Core.World                      | Teleport between worlds.                        |
| /tnt                  | Core.TNT                        | Spawn an active TNT entity.                     |
| /tps                  | Core.Tps                        | Get the current server TPS.                     |
| /save-all             | Core.Save                       | Save all chunks.                                |
| /gamemode             | Core.Gamemode                   | Change player gamemode.                         |
| /stopserver           | Core.StopServer                 | Stops the server.                               |
| /time                 | Core.Time                       | Gets the current time.                          |
| /settime              | Core.Time                       | Allows you to set the current time.             |
| /toggledownfall       | Core.Toggledownfall             | Toggles downfall                                |
| /msg                  | Core.Msg                        | Message a player,                               |
| /tp                   | Core.TP                         | Teleports a player to another player.           |
| /me                   | Core.Me                         | Says something in the third person perspective. |
| /broadcast            | Core.Broadcast                  | Broadcasts a message to all players.            |
| /kick                 | Core.Kick                       | Allows the kicking of a player.                 |

Made possible by
------------------
  - <a href="http://wiki.vg/">Wiki.VG</a> for providing Protocol information.<br>
  - All people that helped me!<br><br>
  - A special thanks to <a href="https://github.com/NiclasOlofsson/">Niclas Olofsson</a>.

Licensing
----------
SharpMC (including SharpCore) uses the permissive MIT license.<br><br>

In a nutshell:<br>
You are not restricted on usage of SharpMC; commercial, private, etc, all fine.<br>
The developers are not liable for what you do with it.<br>
SharpMC is provided "as is" with no warranty.<br>
