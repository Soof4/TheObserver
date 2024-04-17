using System.Drawing.Imaging;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TheObserver;

[ApiVersion(2, 1)]
public class TheObserver : TerrariaPlugin
{
    public override string Name => "The Observer";
    public override string Description => "Detects unusual item activities";
    public override string Author => "Soofa";
    public override Version Version => new Version(0, 1, 5);
    public TheObserver(Main game) : base(game) { }
    public override void Initialize()
    {
        GetDataHandlers.PlayerSlot += OnPlayerSlot;
    }

    private static Dictionary<int, (int, DateTime)> RecentSuspiciousPlayers = new Dictionary<int, (int, DateTime)>();

    private static void OnPlayerSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs args)
    {
        if (args.Player.Active && args.Player.Group.Name != TShock.Config.Settings.DefaultGuestGroupName && !args.Player.HasPermission("theobserver.ignore") && IsSus(args.Type, args.Stack))
        {
            if (RecentSuspiciousPlayers.ContainsKey(args.Player.Index))
            {
                if (RecentSuspiciousPlayers[args.Player.Index].Item1 == args.Type &&
                (DateTime.UtcNow - RecentSuspiciousPlayers[args.Player.Index].Item2).TotalMinutes < 5)
                {
                    return;
                }

                RecentSuspiciousPlayers.Remove(args.Player.Index);
            }

            RecentSuspiciousPlayers.Add(args.Player.Index, (args.Type, DateTime.UtcNow));
            TSPlayer.All.SendMessage($"[The Observer] > {args.Player.Name} has [i/s{args.Stack},p/{args.Prefix}:{args.Type}]", 220, 40, 40);
        }
    }

    private static bool IsSus(int type, int stack)
    {
        return !IsAmmo(type) && CalculateSlotValue(type, stack) > GetProgressFactor() * 2500;
    }

    private static int CalculateSlotValue(int type, int stack)
    {
        int dupePowFactor = stack == 255 || stack == 9999 ? 2 : 1;
        double value = Math.Pow(ContentSamples.ItemsByType[type].rare, dupePowFactor);
        value = Math.Abs(value * stack);

        return (int)value;
    }

    private static bool IsAmmo(int type)
    {
        return ContentSamples.ItemsByType[type].ammo != 0;
    }

    private static int GetProgressFactor()
    {
        if (NPC.downedMoonlord)
        {
            return 11;
        }
        if (NPC.downedAncientCultist)
        {
            return 10;
        }
        if (NPC.downedGolemBoss)
        {
            return 9;
        }
        if (NPC.downedPlantBoss)
        {
            return 8;
        }
        if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
        {
            return 7;
        }
        if (NPC.downedMechBossAny)
        {
            return 6;
        }
        if (Main.hardMode)
        {
            return 5;
        }
        if (NPC.downedBoss3)
        {
            return 4;
        }
        if (NPC.downedBoss2)
        {
            return 3;
        }
        if (NPC.downedBoss1)
        {
            return 2;
        }
        return 1;
    }
}
