using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Microsoft.Xna.Framework;

namespace ProtectedMobs
{
    [ApiVersion(2, 1)]
    public class ProtectedMobs : TerrariaPlugin
    {
        public override string Name => "Protected mobs";
        public override Version Version => new Version(1, 0, 0);
        public override string Author => "Geolindrag";
        public override string Description => "Kills the closest player if a specific mobs is killed by anything";
        //Json Reading stuff
        public static string path = Path.Combine(TShock.SavePath + "/ProtectedMobs.json");
        public static Config Config = new Config();
        //
        public ProtectedMobs(Main game) : base(game){}

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;
            ServerApi.Hooks.NpcKilled.Register(this, OnKill);
            if (File.Exists(path))
                Config = Config.Read();
            else
                Config.Write();
        }

        private void OnKill(NpcKilledEventArgs args)
        {
            TSPlayer player = TShock.Players[args.npc.FindClosestPlayer()];
            for (int i = 0; i < Config.ProtectedMob.Length; i++)
            {
                if (args.npc.netID == Config.ProtectedMob[i])
                {
                    if (!player.Dead)
                    {
                        player.KillPlayer();
                        player.SendMessage(Config.KilledMessage, new Color { R = Config.Red, B = Config.Blue, G = Config.Green });
                    }
                        break;   
                }
            }
        }
        private void OnReload(ReloadEventArgs e)
        {
            if (File.Exists(path))
                Config = Config.Read();
            else
                Config.Write();
        }
    }
}