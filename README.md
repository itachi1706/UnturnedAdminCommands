Admin Commands For Unturned
=====



This is the Admin server management commands for servers running Unturned

**(using `<number>` asks for confirmation, because player numbers seem basically random)**


 Command List | What the command does | Required Permission Level
 --------------------- | ------------------------------------- | -----
 `/ban <player name>/<number>` | Bans a player from the server | 1
 `/kick <player name>/<number>` | Kicks a player from the server | 1
 `/unban <playername>` | Unbans a player from the server | 1
 `/reason <reason>` | Define the reason a player will receive when being kicked/banned | 1
 `/repeat <announcement>` | Broadcast a server wide announcement | 1
 `/repairvehicles` | Repairs all vehicles on the server | 1
 `/refuelvehicles` | Refuels all vehicles on the server | 1
 `/tp <player name>/<number>` | Teleport to a player | 1
 `/tptome <player name>/<number>` | Teleports a player to you | 1
 `/tpall` | Teleports all players to you | 2
 `/resetzombies` | Resets and respawns all zombies on the server | 2
 `/resetitems` | Removes and respawns all items on the server | 2
 `/kill <playername>` | Kills a player | 2
 `/enablewhitelist` | Enables whitelisting | 3
 `/disablewhitelist` | Disable whitelisting | 3
 `/setannouncedelay` | Set Announcement Delay | 3
 `/setitemsdelay <time in seconds>` | Sets the item spawn delay in seconds | 4
 `/reloadbans` | Reloads Ban List | 4
 `/reloadCommands` | Reloads the utility | 4
 
 **Permission Level System**
 
 Level | Description
 ----- | -----------
 1 | Basic Moderation Commands + Vehicle Management
 2 | Trusted Moderators, able to kill/respawn items
 3 | Administrators, able to enable/disable whitelist
 4 | Global Administrators, able to reload the utility
 
 
 **UnturnedAdmins.txt Format**
 ```
 <steamname>:<steam base64 id>:<permission level>
 e.g.
 itachi1706:76561198086011973:4
 ```