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
    public override Version Version => new Version(0, 1, 6);
    public TheObserver(Main game) : base(game) { }
    public override void Initialize()
    {
        GetDataHandlers.PlayerSlot += OnPlayerSlot;

        Commands.ChatCommands.Add(new Command("theobserver.rate", RateCmd, "rate"));

        Data.Read();
    }

    private static void RateCmd(CommandArgs args)
    {
        int rating;

        if (args.Parameters.Count == 0 || !int.TryParse(args.Parameters[0], out rating))
        {
            args.Player.SendErrorMessage("Please choose a number between 0 and 10. (10 meaning this report was spot on right, 0 meaning it was an awful report.)");
            return;
        }

        rating = rating > 10 ? 10 : (rating < 0 ? 0 : rating);    // if rating is lesser than 0 then set it as 0, if it's greater than 10 then set it as 10
        double trainingValue = GetTrainingValueFromRating(rating);
        UpdateCurrentValue(GetProgressFactor() + trainingValue);
    }

    private static double GetTrainingValueFromRating(int rating)
    {
        return -0.002 * rating + 0.01;
    }

    private static void UpdateCurrentValue(double newValue)
    {
        if (NPC.downedMoonlord)
        {
            Data.PostML = newValue;
        }
        else if (NPC.downedAncientCultist)
        {
            Data.PostCultist = newValue;
        }
        else if (NPC.downedGolemBoss)
        {
            Data.PostGolem = newValue;
        }
        else if (NPC.downedPlantBoss)
        {
            Data.PostPlantera = newValue;
        }
        else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
        {
            Data.PostAllMech = newValue;
        }
        else if (NPC.downedMechBossAny)
        {
            Data.PostAnyMech = newValue;
        }
        else if (Main.hardMode)
        {
            Data.PostWOF = newValue;
        }
        else if (NPC.downedBoss3)
        {
            Data.PostSkeletron = newValue;
        }
        else if (NPC.downedBoss2)
        {
            Data.PostEvilBoss = newValue;
        }
        else if (NPC.downedBoss1)
        {
            Data.PostEOC = newValue;
        }
        else
        {
            Data.PreBossValue = newValue;
        }


        Data.Write();
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

    private static double GetProgressFactor()
    {
        if (NPC.downedMoonlord)
        {
            return Data.PostML;
        }
        if (NPC.downedAncientCultist)
        {
            return Data.PostCultist;
        }
        if (NPC.downedGolemBoss)
        {
            return Data.PostGolem;
        }
        if (NPC.downedPlantBoss)
        {
            return Data.PostPlantera;
        }
        if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
        {
            return Data.PostAllMech;
        }
        if (NPC.downedMechBossAny)
        {
            return Data.PostAnyMech;
        }
        if (Main.hardMode)
        {
            return Data.PostWOF;
        }
        if (NPC.downedBoss3)
        {
            return Data.PostSkeletron;
        }
        if (NPC.downedBoss2)
        {
            return Data.PostEvilBoss;
        }
        if (NPC.downedBoss1)
        {
            return Data.PostEOC;
        }
        return Data.PreBossValue;
    }
}
