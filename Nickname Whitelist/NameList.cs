using System;
using System.IO;
using TerrariaApi.Server;
using Terraria;
using TShockAPI;


namespace NameList
{
    [ApiVersion(2, 1)]
    public class NameWhiteList : TerrariaPlugin
    {
        internal static string WhitelistPath
        {
            get { return Path.Combine(TShock.SavePath, "whitelist.txt"); }
        }
        public override string Author
        {
            get { return "Bokmako"; }
        }
        public override string Description
        {
            get { return "Whitelist by nicknames"; }
        }

        public override string Name
        {
            get { return "NameWhiteList"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }
        public NameWhiteList(Main game)
            : base(game)
        {
            Order = 0;
        }
        public override void Initialize()
        {
            ServerApi.Hooks.ServerJoin.Register(this, WLMainn);
        }
        public void WLMainn(JoinEventArgs args)
        {
            var player = new TSPlayer(args.Who);
            Console.WriteLine(player.Name);
            if (!OnWhitelist(player.Name))
            {
                player.Kick("You are not on the whitelist.", true, true, null, false);
                Console.WriteLine("Игрок {0} пытался зайти без спроса", player.Name);
                args.Handled = true;
                return;
            }
        }

        public static void CreateIfNot(string file, string data = "")
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, data);
            }
        }
        public static bool OnWhitelist(string name)
        {
            CreateIfNot(WhitelistPath, "Billy");
            using (var tr = new StreamReader(WhitelistPath))
            {
                string whitelist = tr.ReadToEnd();
                bool contains = whitelist.Contains(name);
                if (!contains)
                {
                    foreach (var line in whitelist.Split(Environment.NewLine.ToCharArray()))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        contains = TShock.Utils.GetIPv4AddressFromHostname(line).Equals(name);
                        if (contains)
                            return true;
                    }
                    return false;
                }
                return true;
            }
        }
    }
}