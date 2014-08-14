Admin Commands For Unturned
=====



This is the Admin server management commands for servers running Unturned

**(using `<number>` asks for confirmation, because player numbers seem basically random)**


 Command List | What the command does | Required Permission Level
 --------------------- | ------------------------------------- | -----
 `/time` | Shows the current time on the server | 0
 `/online` | List the number of players currently online | 0
 `/ban <player name>/<number>` | Bans a player from the server | 1
 `/kick <player name>/<number>` | Kicks a player from the server | 1
 `/unban <player name>` | Unbans a player from the server | 1
 `/reason <reason>` | Define the reason a player will receive when being kicked/banned | 1
 `/repeat <announcement>` | Broadcast a server wide announcement | 1
 `/repairvehicles` | Repairs all vehicles on the server | 1
 `/refuelvehicles` | Refuels all vehicles on the server | 1
 `/car` | Teleports a random car from anywhere in the map to you | 1
 `/sirens` | Enable sirens on all vehicles that supports it (Horns too maybe) | 1
 `/heal <player name>` | Heals a player | 1
 `/tp <player name>/<number>` | Teleport to a player | 1
 `/tptome <player name>/<number>` | Teleports a player to you | 1
 `/tpall` | Teleports all players to you | 2
 `/killzombies` | Kills all of the zombies on the map | 2
 `/resetzombies` | Resets and respawns all zombies on the server | 2
 `/resetitems` | Removes and respawns all items on the server | 2
 `/i <item id> <amount>` | Spawns a specific item for the player (Check below for the list of item ids) | 2
 `/kit` | Spawns a kit for the player | 2
 `/kill <playername>` | Kills a player | 2
 `/enablewhitelist` | Enables whitelisting | 3
 `/disablewhitelist` | Disable whitelisting | 3
 `/setannouncedelay` | Set Announcement Delay | 3
 `/setitemsdelay <time in seconds>` | Sets the item spawn delay in seconds | 4
 `/reloadbans` | Reloads Ban List | 4
 `/reloadCommands` | Reloads the utility | 4
 `/promote <player name>` | Promotes a player to Admin Lvl 1 (Moderator) | 4
 `/logmsg` | *DEBUG* Logs a message to CONSOLE | 4
 
 **Permission Level System**
 *Each level inherits the commands of the previous level*
 Level | Rank | Description
 ----- | ---- | -----------
 0 | Normal | Normal User
 1 | Moderator | Standard Mod commands + Vehicle Management
 2 | Trusted Moderator | Able to kill/reset/spawn items/mobs
 3 | Administrators | Able to set delay for announcements and whitelist management
 4 | OPs/Global Administrators | Able to log to console/reset utility
 
 
 **UnturnedAdmins.txt Format**
 ```
 <steamname>:<steam base64 id>:<permission level>
 e.g.
 itachi1706:76561198086011973:4
 ```
 **Item IDs**
 http://unturned.wikia.com/wiki/Item_ID%27s