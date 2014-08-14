using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Steamworks;
using System.Reflection;
using System.Timers;

namespace Admin_Commands
{

    internal class AdminCommands : MonoBehaviour
    {

        public String playerName = "";
        public String playerID;
        
        public float updater;
        public float updater2;
        public float updater3;


        private Vector2 scrollViewVector = Vector2.zero;
        public Rect dropDownRect = new Rect(100, 40, 325, 300);
        public string reason = "You were kicked from the server";
        public static string[] names = { "PLAYER NAMES HERE :D" };
        public static string[] ids = { "PLAYER STEAMIDS HERE :D" };
        public string lastUsedCommand = "none";
        public string tempKickName = "";
        public string tempBanName = "";

        public Timer itemsTimer;
        public Timer announceTimer;

        public int announceIndex = 0;
        public int itemsResetIntervalInSeconds = 2700;
        public int announceIntervalInSeconds = 600;
        public String[] AnnounceMessages;

        public String[] WhitelistedPlayers;
        public bool usingWhitelist = false;


        private String[] AdminNames;
        private String[] AdminSteamIDs;

        public String bigAssStringWithBannedPlayerNamesAndSteamIDs = "";   //empty until player issues /unban command

        /*
         * Administrative Permission Level
         * 0: For all users (online, time)
         * 1: Moderation Commands (ban,kick,unban,tp,tptome, repeat, reason, repairVehicles, refuelVehicles, car, heal, sirens)
         * 2: Trusted Moderation Commands (tpall, kill, resetZombies, resetItems, i, killzombies, kit)
         * 3: Admin Commands (enableWhiteList, disableWhiteList, setannouncedelay)
         * 4: OP (setItemDelay, reloadCommands, reloadBans, promote, logmsg)
         */
        private Int32[] AdminPermissionLevel;
        

        public void Start()
        {
            Directory.CreateDirectory("Unturned_Data/Managed/mods/AdminCommands");

            if (!File.Exists("Unturned_Data/Managed/mods/AdminCommands/UnturnedAdmins.txt"))  //create a template for admins
            {
                System.IO.StreamWriter file = new StreamWriter("Unturned_Data/Managed/mods/AdminCommands/UnturnedAdmins.txt", true);
                file.WriteLine("Nessin:76561197976976379:4");
                file.WriteLine("Some Other Admin:12345789:4");

                file.Close();
            }
            string[] lines = System.IO.File.ReadAllLines(@"Unturned_Data/Managed/mods/AdminCommands/UnturnedAdmins.txt");
            AdminNames = new String[lines.Length];
            AdminSteamIDs = new String[lines.Length];
            AdminPermissionLevel = new Int32[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 10)
                {
                    AdminNames[i] = lines[i].Split(':')[0];
                    AdminSteamIDs[i] = lines[i].Split(':')[1];
                    try { 
                        AdminPermissionLevel[i] = Convert.ToInt32(lines[i].Split(':')[2]);
                    }
                    catch (System.Exception)
                    {
                        AdminPermissionLevel[i] = 4;
                    }
                }
            }

            //WHITELIST
            if (!File.Exists("Unturned_Data/Managed/mods/AdminCommands/UnturnedWhitelist.txt"))  //create a template for whitelist
            {
                System.IO.StreamWriter file = new StreamWriter("Unturned_Data/Managed/mods/AdminCommands/UnturnedWhitelist.txt", true);
                file.WriteLine("Nessin");
                file.WriteLine("Some other player");
                file.Close();
            }
            string[] whitelisteds = System.IO.File.ReadAllLines(@"Unturned_Data/Managed/mods/AdminCommands/UnturnedWhitelist.txt");
            WhitelistedPlayers = new String[whitelisteds.Length];
            for (int i = 0; i < whitelisteds.Length; i++)
            {
                WhitelistedPlayers[i] = whitelisteds[i];
            }



            if (!File.Exists("Unturned_Data/Managed/mods/AdminCommands/UnturnedAnnounces.txt"))  //create a template for announcements
            {
                System.IO.StreamWriter file = new StreamWriter("Unturned_Data/Managed/mods/AdminCommands/UnturnedAnnounces.txt", true);
                file.WriteLine("This line will be announced 10 minutes after injecting");
                file.WriteLine("This line will be announced at the same time");
                file.WriteLine(":");
                file.WriteLine("This line will be announced 20 minutes after injecting");
                file.WriteLine(":");
                file.WriteLine("This line will be announced 30 minutes after injecting");
                file.WriteLine("And so forth.. then it will go back to the 1st line");
                file.Close();
            }
            string[] announces = System.IO.File.ReadAllLines(@"Unturned_Data/Managed/mods/AdminCommands/UnturnedAnnounces.txt");
            AnnounceMessages = new String[announces.Length];
            for (int i = 0; i < announces.Length; i++)
            {
                AnnounceMessages[i] = announces[i];
            }

            itemsTimer = new Timer(itemsResetIntervalInSeconds * 1000);
            itemsTimer.Elapsed += new ElapsedEventHandler(this.itemsTimeElapsed);
            itemsTimer.Enabled = true;

            announceTimer = new Timer(announceIntervalInSeconds * 1000);
            announceTimer.Elapsed += new ElapsedEventHandler(this.announcesTimeElapsed);
            announceTimer.Enabled = true;


            //Screen.showCursor = true;


        }

        public void announcesTimeElapsed(object sender, ElapsedEventArgs e)
        {
            announceNext();
        }


        private void announceNext()
        {
            for (int i = announceIndex; i < AnnounceMessages.Length; i++)
            {
                string message = AnnounceMessages[i];
                if (message.Equals(":"))
                {
                    announceIndex = i + 1;
                    return;
                }
                else
                {
                    NetworkChat.sendAlert(message);
                }
            }
            announceIndex = 0;
        }

        public void setAnnounceIntervalInSeconds(int seconds)
        {
            announceIntervalInSeconds = seconds;
            announceTimer.Stop();
            announceTimer.Interval = seconds * 1000;
            announceTimer.Start();
        }

        private void itemsTimeElapsed(object sender, ElapsedEventArgs e)
        {
            resetItems();
        }

        public void setItemResetIntervalInSeconds(int seconds)
        {
            itemsResetIntervalInSeconds = seconds;
            itemsTimer.Stop();
            itemsTimer.Interval = seconds * 1000;
            itemsTimer.Start();
        }

        public void reloadCommands()
        {
            itemsTimer.Stop();
            itemsTimer.Dispose();
            announceTimer.Stop();
            announceTimer.Dispose();
            Start();

        }

        public void resetItems()
        {
            SpawnItems.reset();
        }

        private void UpdatePlayerList(bool forceUpdate)
        {
            if (this.updater <= 1f || forceUpdate == true)
            {
                Player[] players = UnityEngine.Object.FindObjectsOfType(typeof(Player)) as Player[];
                names = new String[players.Length];
                ids = new String[players.Length];
                FieldInfo[] fis = typeof(NetworkUser).GetFields();
                for (int i = 0; i < players.Length; i++)
                {
                    names[i] = players[i].name;

                    NetworkPlayer np = players[i].networkView.owner;
                    NetworkUser nu = NetworkUserList.getUserFromPlayer(np);

                    int num = 0;


                    foreach (FieldInfo fi in fis)
                    {
                        if (num == 3)
                        {
                            try
                            {
                                ids[i] = fi.GetValue(nu).ToString();
                                break;
                            }
                            catch (Exception)
                            {

                            }
                        }
                        num++;
                    }
                }
                if (names.Length == 0)
                {
                    names = new String[1];
                    ids = new String[1];
                }
                this.updater = 500f;
            }
        }



        private String getLastMessagePlayerName()
        {
            return getMsgByNum(3);
        }

        private String getLastMessageText()
        {
            return getMsgByNum(6);
        }

        private String getLastMessageRep()
        {
            return getMsgByNum(9);
        }

        public void resetChat()
        {
            String[] texts = new String[4];
            texts[0] = getMsgByNum(13);
            texts[1] = getMsgByNum(20);
            texts[2] = getMsgByNum(27);
            texts[3] = getMsgByNum(34);

            for (int i = 0; i < 4; i++)
            {
                if (texts[i].StartsWith("/")) {
                    texts[i] = texts[i].Substring(1);
                }
            }

            NetworkPlayer[] players = new NetworkPlayer[4];
            bool[] isServerMsg = new bool[4];
            String[] names = new String[4];

            names[0] = getMsgByNum(10);
            names[1] = getMsgByNum(17);
            names[2] = getMsgByNum(24);
            names[3] = getMsgByNum(31);

            for (int i = 0; i < 4; i++)
            {
                players[i] = getNetworkPlayerByPlayerName(names[i]);
                if (names[i].StartsWith("Server"))
                {
                    isServerMsg[i] = true;
                }
            }

            if (!isServerMsg[3])
            {
                getNetworkChat().askChat(texts[3], 0, players[3]);
            }
            else
            {
                NetworkChat.sendAlert(texts[3]);
            }
            for (int i = 0; i < 4; i++)
            {
                if (!isServerMsg[3-i])
                {
                    getNetworkChat().askChat(texts[3 - i], 0, players[3 - i]);
                }
                else
                {
                    NetworkChat.sendAlert(texts[3-i]);

                }
            }




        }

        private String getMsgByNum(int num2)
        {
            NetworkChat[] list = UnityEngine.Object.FindObjectsOfType(typeof(NetworkChat)) as NetworkChat[];
            int num = 0;
            FieldInfo[] fis = typeof(NetworkChat).GetFields();
            foreach (NetworkChat nu in list)
            {
                //Log("NetworkChat:" + num);
                foreach (FieldInfo fi in fis)
                {
                    if (num == num2)
                    {
                        try
                        {
                            return (fi.GetValue(nu).ToString());
                        }
                        catch (Exception)
                        {

                        }
                    }
                    num++;
                }

            }
            return "";
        }

        private NetworkChat getNetworkChat()
        {
            NetworkChat chat = UnityEngine.Object.FindObjectOfType(typeof(NetworkChat)) as NetworkChat;
            return chat;
        }

        private void Log(string p)
        {
            System.IO.StreamWriter file = new StreamWriter("Unturned_Data/Managed/mods/AdminCommands_Log.txt", true);
            file.WriteLine(p);

            file.Close();
        }

        public String getSteamIDByPlayerName(String playerNaam)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Equals(playerNaam))
                {
                    return ids[i];
                }
            }
            return "";
        }

        public NetworkPlayer getNetworkPlayerByPlayerName(String playerNaam)
        {
            Player[] players = GameObject.FindObjectsOfType<Player>();
            NetworkPlayer np = players[0].networkView.owner;
            foreach (Player p in players)
            {
                if (p.name.Equals(playerNaam))
                {
                    return p.networkView.owner;
                }
            }
            return np;
        }

        private bool isAdmin(String name)
        {
            UpdatePlayerList(true);
            for (int i = 0; i < AdminNames.Length; i++)
            {
                if (AdminNames[i].Equals(name) && AdminSteamIDs[i].Equals(getSteamIDByPlayerName(name)))
                {

                    return true;
                }
            }
            return false;
        }

        private int getAdminLevel(String name)
        {
            UpdatePlayerList(true);
            for (int i = 0; i < AdminNames.Length; i++)
            {
                if (AdminNames[i].Equals(name) && AdminSteamIDs[i].Equals(getSteamIDByPlayerName(name)))
                {
                    return AdminPermissionLevel[i];
                }
            }
            return -1;
        }

        private void kickFakeAdmins()
        {
            if (updater2 <= 1f)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (AdminNames.Contains(names[i]))
                    {
                        if (!isAdmin(names[i]))
                        {
                            playerName = names[i];
                            reason = "Change your name";
                            KICK();
                            reason = "You were kicked from the server";
                        }
                    }
                }
                this.updater2 = 500f;
            }
        }

        private void kickNonWhitelistedPlayers()
        {
            if (usingWhitelist && updater3 <= 1f)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (!WhitelistedPlayers.Contains(names[i]))
                    {
                        KICK(names[i], "You are not whitelisted on this server!");
                    }
                }
                this.updater3 = 100f;
            }
        }

        public void loadBans()
        {
            bigAssStringWithBannedPlayerNamesAndSteamIDs = PlayerPrefs.GetString("bans");
        }

        public void saveBans()
        {
            PlayerPrefs.SetString("bans", bigAssStringWithBannedPlayerNamesAndSteamIDs);
        }

        public void unban(String name)
        {
            loadBans();
            string bannedppl = bigAssStringWithBannedPlayerNamesAndSteamIDs;

            if (bannedppl.Contains(name))
            {
                int startIndex = bannedppl.IndexOf(name);
                int length = name.Length + 1 + 17 + 2;

                /*NetworkChat.sendAlert("startindex = " +startIndex);
                NetworkChat.sendAlert("length = " + length);

                //bannedppl.Remove(startIndex, length);    // NO IDEA WHY THE FUCK THIS WONT WORK  */

                String temp1 = bannedppl.Substring(0, startIndex);
                String temp2 = bannedppl.Substring(startIndex + length);

                bannedppl = temp1 + temp2;
            }

            bigAssStringWithBannedPlayerNamesAndSteamIDs = bannedppl;
            saveBans();
            NetworkBans.load();
        }

        void BAN()
        {
            Player[] players = GameObject.FindObjectsOfType<Player>();
            foreach (Player p in players)
            {
                if (p.name.Equals(playerName))
                {
                    NetworkPlayer np = p.networkView.owner;
                    NetworkTools.ban(np, playerName, playerID, reason);
                    return;
                }
            }
        }

        void KICK(String name, String reason)
        {
            Player[] players = GameObject.FindObjectsOfType<Player>();
            foreach (Player p in players)
            {
                if (p.name.Equals(name))
                {
                    NetworkPlayer np = p.networkView.owner;
                    NetworkTools.kick(np, reason);
                    return;
                }
            }
        }

        void KICK()
        {
            Player[] players = GameObject.FindObjectsOfType<Player>();
            foreach (Player p in players)
            {
                if (p.name.Equals(playerName))
                {
                    NetworkPlayer np = p.networkView.owner;
                    NetworkTools.kick(np, reason);
                    return;
                }
            }
        }

        public void Update()
        {
            UpdatePlayerList(false);
            kickFakeAdmins();
            kickNonWhitelistedPlayers();
            updater--;
            updater2--;
            updater3--;

            checkForCommands();

            //Check last (bottom) message for commands


        }

        public void checkForCommands()
        {
            if (getLastMessageText().StartsWith("/"))
            {
                String sender = getLastMessagePlayerName();
                String commando = getLastMessageText();
                resetChat();
                //NetworkChat.sendAlert(""); //avoid looping into commands
                lastUsedCommand = commando;

                if (isAdmin(sender))
                {
                    if (getAdminLevel(sender) == -1)
                    {
                        getNetworkChat().askChat("An error occured! You appear to not have Admin Rights!", 2, getNetworkPlayerByPlayerName(sender));
                    }
                    else
                    {
                        int permLvl = getAdminLevel(sender);
                        if (commando.StartsWith("/repeat") && permLvl >= 1)
                        {
                            NetworkChat.sendAlert(commando.Substring(8));
                        }

                        else if (commando.StartsWith("/ban") && permLvl >= 1)
                        {
                            String naam = commando.Substring(5);
                            if (naam.Length < 3)
                            {
                                naam = names[Convert.ToInt32(naam)];
                                getNetworkChat().askChat("Ban " + naam + " ?  /yy to confirm", 2, getNetworkPlayerByPlayerName(sender));
                                tempBanName = naam;
                            }
                            else
                            {
                                playerID = getSteamIDByPlayerName(naam);
                                if (!playerID.Equals(""))
                                {
                                    playerName = naam;
                                    BAN();
                                }
                            }

                        }
                        else if (commando.StartsWith("/kick") && permLvl >= 1)
                        {
                            String naam = commando.Substring(6);
                            if (naam.Length < 3)
                            {
                                naam = names[Convert.ToInt32(naam)];
                                getNetworkChat().askChat("Kick " + naam + " ?  /y to confirm", 2, getNetworkPlayerByPlayerName(sender));
                                tempKickName = naam;
                            }
                            else
                            {
                                playerName = naam;
                                KICK();
                            }

                        }
                        else if (commando.Equals("/y"))
                        { // kick
                            playerName = tempKickName;
                            KICK();
                        }
                        else if (commando.Equals("/yy"))
                        { //ban
                            playerID = getSteamIDByPlayerName(tempBanName);
                            if (!playerID.Equals(""))
                            {
                                playerName = tempBanName;
                                BAN();
                            }
                        }
                        else if (commando.StartsWith("/reason") && permLvl >= 1)
                        {
                            reason = commando.Substring(8);
                        }
                        else if (commando.StartsWith("/resetitems") && permLvl >= 2)
                        {
                            SpawnItems.reset();
                            NetworkChat.sendAlert(sender + " has respawned all items");
                        }
                        else if (commando.StartsWith("/repairvehicles") && permLvl >= 1)
                        {
                            Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                            foreach (Vehicle vehicle in vehicles)
                            {
                                vehicle.tellExploded(false);
                                vehicle.heal(1000);
                            }
                            NetworkChat.sendAlert(sender + " has repaired all vehicles");
                        }
                        else if (commando.StartsWith("/refuelvehicles") && permLvl >= 1)
                        {
                            Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                            foreach (Vehicle vehicle in vehicles)
                            {
                                vehicle.fill(1000);
                            }
                            NetworkChat.sendAlert(sender + " has refueled all vehicles");
                        }
                        else if (commando.StartsWith("/sirens") && permLvl >= 1)
                        {
                            Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                            foreach (Vehicle vehicle in vehicles)
                            {
                                vehicle.tellSirens(true);
                            }
                        }

                        else if (commando.StartsWith("/resetzombies") && permLvl >= 2)
                        {
                            SpawnAnimals.reset();
                            NetworkChat.sendAlert(sender + " has respawned all zombies");
                        }
                        else if (commando.StartsWith("/killzombies") && permLvl >= 2)
                        {
                            Zombie[] Zombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
                            foreach (Zombie Zombie in Zombies)
                            {
                                Zombie.damage(500);
                            }
                            NetworkChat.sendAlert(sender + " has killed all zombies");
                        }
                        else if (commando.StartsWith("/reloadbans") && permLvl >= 3)
                        {
                            NetworkBans.load();
                        }
                        else if (commando.StartsWith("/setitemsdelay") && permLvl == 4)
                        {
                            String seconds = commando.Substring(15);
                            setItemResetIntervalInSeconds(Convert.ToInt32(seconds));
                        }
                        else if (commando.StartsWith("/setannouncedelay") && permLvl >= 3)
                        {
                            String seconds = commando.Substring(18);
                            setAnnounceIntervalInSeconds(Convert.ToInt32(seconds));
                        }
                        else if (commando.StartsWith("/reloadCommands") && permLvl >= 4)
                        {
                            reloadCommands();
                        }
                        else if (commando.StartsWith("/logmsg") && permLvl >= 4)
                        {
                            for (int i = 0; i < 80; i++)
                            {
                                Log(getMsgByNum(i));
                            }
                        }



                        else if (commando.StartsWith("/enablewhitelist") && permLvl >= 3)
                        {
                            usingWhitelist = true;
                        }
                        else if (commando.StartsWith("/disablewhitelist") && permLvl >= 3)
                        {
                            usingWhitelist = false;
                        }
                        else if (commando.StartsWith("/unban") && permLvl >= 1)
                        {
                            String name = commando.Substring(7);
                            unban(name);

                        }
                        else if (commando.StartsWith("/tptome") && permLvl >= 1)
                        {
                            String name = commando.Substring(8);
                            if (name.Length < 3)
                            {
                                name = names[Convert.ToInt32(name)];
                            }
                            //big ass line incoming
                            NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(name)).transform.position = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.position;
                            NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(name)).GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, new object[] { NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.position, NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.rotation });
                        }

                        else if (commando.Equals("/tpall") && permLvl >= 2)
                        {
                            Vector3 location = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.position;
                            Quaternion rotation = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.rotation;
                            foreach (String name in names)
                            {

                                //There's probably a shorter way to this teleport stuff but hey this works xD
                                NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(name)).transform.position = location;
                                NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(name)).GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, new object[] { location, rotation });
                            }
                        }

                        else if (commando.StartsWith("/tp") && permLvl >= 1)  //make sure this goes under /tptome
                        {
                            String name = commando.Substring(4);
                            if (name.Length < 3)
                            {
                                name = names[Convert.ToInt32(name)];
                            }
                            NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.position = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(name)).transform.position;
                            NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, new object[] { NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(name)).transform.position, NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(name)).transform.rotation });

                        }

                        else if (commando.StartsWith("/kill") && permLvl >= 2)
                        {
                            //All of these things are buggy as fuck
                            String naam = commando.Substring(6);
                            // NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(naam)).GetComponent<Life>().tellAllLife(10,0,0,0,true,true);
                            // NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(naam)).GetComponent<Life>().tellDead(true, "You were shot in the face with a rocket launcher");
                            NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(naam)).GetComponent<Life>().damage(500, "You were struck down by the Wrath of the Gods!!!");
                        }

                        else if (commando.StartsWith("/heal") && permLvl >= 1)
                        {
                            //All of these things are buggy as fuck
                            String naam = commando.Substring(6);
                            // NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(naam)).GetComponent<Life>().tellAllLife(10,0,0,0,true,true);
                            // NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(naam)).GetComponent<Life>().tellDead(true, "You were shot in the face with a rocket launcher");
                            NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(naam)).GetComponent<Life>().heal(100, true, true);
                        }


                        else if (commando.StartsWith("/promote") && permLvl >= 4)
                        {
                            String naam = commando.Substring(9);

                            System.IO.StreamWriter file = new StreamWriter("Unturned_Data/Managed/mods/AdminCommands/UnturnedAdmins.txt", true);
                            file.WriteLine("");
                            file.WriteLine(naam + ":" + getSteamIDByPlayerName(naam) + ":1");
                            file.Close();
                            NetworkChat.sendAlert(naam + " was promoted to the role of Moderator. (Level 1 Admin)");

                            reloadCommands();
                        }
                        else if (commando.StartsWith("/car") && permLvl >= 1)
                        {
                            Vector3 location = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.position;
                            Quaternion rotation = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.rotation;

                            Vehicle[] mapVehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                            int random = UnityEngine.Random.Range(0, 10);
                            Vehicle randomVehicle = mapVehicles[random];
                            Vector3 newPos = new Vector3(location[0] + 5, location[1] + 5, location[2]);
                            randomVehicle.updatePosition(newPos, rotation);
                            randomVehicle.transform.position = newPos;
                        }


                        else if (commando.StartsWith("/i ") && permLvl >= 2)
                        {
                            int itemid = Convert.ToInt32(commando.Split(' ')[1]);
                            int amount = 1;
                            if (commando.Split(' ').Length > 2)
                            {
                                amount = Convert.ToInt32(commando.Split(' ')[2]);
                            }

                            Vector3 location = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.position;

                            for (int i = 0; i < amount; i++)
                                SpawnItems.spawnItem(itemid, 1, location);

                        }



                        else if (commando.Equals("/kit") && permLvl >= 2) 
                        {
                            int[] itemids = new int[] { 0x7d4, 0x1b60, 0x2ee0, 0x232c, 0x2711, 0x2afb, 0x465e, 0x465e, 0x465e, 0x465e, 0x465e, 0x465e, 0x465e, 0xfb1, 0x1399, 11, 0x32c8, 0x32c8, 0x36c6, 0x36c6, 0x1f4f, 0x1f4d, 0xbba };
                            Vector3 location = NetworkUserList.getModelFromPlayer(getNetworkPlayerByPlayerName(sender)).transform.position;

                            foreach (int itemid in itemids)
                                SpawnItems.spawnItem(itemid, 1, location);

                        }

                    }

                } // end of isAdmin()

                else
                {

                    // KICK(sender, "Did you just kick yourself?");

                }


                //commands for non-admins

                    if (commando.Equals("/online"))
                    {
                        getNetworkChat().askChat("There are " + names.Length + " players online.", 2, getNetworkPlayerByPlayerName(sender));

                    }
                    else if (commando.StartsWith("/time"))
                    {
                        NetworkChat.sendAlert("Time: " + Sun.getTime());
                    }


            } //end checkForCommands()
        }

        
        public void OnGUI()
        {
            GUI.BeginGroup(new Rect(50, 100, 600, 70));
            // All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.

            // We'll make a box so you can see where the group is on-screen.
            GUI.Box(new Rect(0, 0, 530, 400), "Admin Commands running! Last command used: " + lastUsedCommand);


            // End the group we started above. 
            GUI.EndGroup();
        }


    }

}