using Microsoft.Xna.Framework;
using On.Terraria;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Main = Terraria.Main;

namespace ProtectedMobs
{
    [ApiVersion(2, 1)]
    public class ProtectedMobs : TerrariaPlugin
    {
        public override string Name => "Protected mobs";
        public override Version Version => new Version(1, 3, 0);
        public override string Author => "Geolindrag";
        public override string Description => "Kills the closest player if a specific mobs is killed by anything";
        //Json Reading stuff
        public static string path = Path.Combine(TShock.SavePath + "/ProtectedMobs.json");
        public static Config Config = new Config();
        //
        public ProtectedMobs(Main game) : base(game) { }

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;
            //Commands.ChatCommands.Add(new Command(Permissions.canchat,))
            //ServerApi.Hooks.NpcKilled.Register(this, OnKill);
            ServerApi.Hooks.NpcStrike.Register(this, OnDamage);
            if (File.Exists(path))
                Config = Config.Read();
            else
                Config.Write();

        }

        private void OnDamage(NpcStrikeEventArgs args)
        {
            TSPlayer player = TSPlayer.FindByNameOrID(args.Player.name.ToString()).First();
            if (args.Damage > args.Npc.life)
            {
                for (int i = 0; i < Config.ProtectedMob.Length; i++)
                {
                    if (args.Npc.netID == Config.ProtectedMob[i])
                    {
                        switch (Config.Mode)
                        {
                            case 0://mode 0,Just Kill
                                if (!player.Dead)
                                {
                                    string Message = Config.KilledMessage;
                                    player.KillPlayer();
                                    if (Config.KilledMessage.Contains("{0}"))
                                    {
                                        Message = Message.Replace("{0}", args.Npc.FullName.ToString());
                                    }
                                    if (Config.KilledMessage.Contains("{1}"))
                                    {
                                        Message = Message.Replace("{1}", player.Name.ToString());
                                    }
                                    player.SendMessage(Message, new Color { R = Config.Red, B = Config.Blue, G = Config.Green });
                                }
                                break;
                            case 1: //Mode 1 , summon Boss/Mob
                                for (int limit = 0; limit < Config.Amt; limit++)
                                {
                                    Terraria.NPC.SpawnBoss(((int)player.X) + (32 * (limit + 1)), ((int)player.Y) + 200, Config.SummonMob, player.Index);
                                }
                                break;
                            case 2: //Mode 2 Create projectile, Intended for explotions mostly
                                Terraria.Projectile.NewProjectile(Terraria.Projectile.GetNoneSource(), args.Npc.VisualPosition.X, args.Npc.VisualPosition.Y, 0, 0, Config.Mode2Proj, 500, 0.0f, -1, 0, 0, 0);
                                break;   
                            default:
                                player.SendMessage("Not valid Mode", new Color { R = 255, B = 50, G = 50 });
                                break;
                        }
                        break;
                    }
                }
            }

        }
        //Legacy Stuff that may be useful, or not
        /* private void OnKill(NpcKilledEventArgs args)
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
         }*/
        private void OnReload(ReloadEventArgs e)
        {
            if (File.Exists(path))
                Config = Config.Read();
            else
                Config.Write();
        }
    }
}
