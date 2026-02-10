using TerrariaParsers.Common;
using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Player;

public class TerrariaPlayerInfo
{
    public class CreativePowerInfo
    {
        public bool GodmodePowerEnabled { get; set; } = false;

        public bool FarPlacementRangePowerEnabled { get; set; } = false;

        public float SpawnRatePowerSliderValue { get; set; } = 0.5f;
    }

    public TerrariaSaveFileInfo FileInfo { get; set; }

    public string Name { get; set; }

    public PlayerCharacterMode Difficulty { get; set; }

    public TimeSpan PlayTime { get; set; }

    public int Hair { get; set; }
    public byte HairDye { get; set; }
    public int Team { get; set; }
    public bool[] HideVisibleAccessory { get; set; } = new bool[10];
    public bool[] HideMisc { get; set; } = new bool[8];
    public bool Male
    {
        get { return PlayerSkinVariantHelpers.IsMaleSkinVariant(SkinVariant); }
        set { }
    }
    public PlayerSkinVariant SkinVariant { get; set; }
    public int StatLife { get; set; }
    public int StatLifeMax { get; set; }
    public int StatMana { get; set; }
    public int StatManaMax { get; set; }
    public bool ExtraAccessory { get; set; }
    public bool UnlockedBiomeTorches { get; set; }
    public bool UsingBiomeTorches { get; set; }
    public bool AteArtisanBread { get; set; }
    public bool UsedAegisCrystal { get; set; }
    public bool UsedAegisFruit { get; set; }
    public bool UsedArcaneCrystal { get; set; }
    public bool UsedGalaxyPearl { get; set; }
    public bool UsedGummyWorm { get; set; }
    public bool UsedAmbrosia { get; set; }
    public bool DungeonDefendersEventCompleted { get; set; }
    public int TaxMoney { get; set; }
    public int NumberOfDeathsPVE { get; set; }
    public int NumberOfDeathsPVP { get; set; }
    public Color HairColor { get; set; }
    public Color SkinColor { get; set; }
    public Color EyeColor { get; set; }
    public Color ShirtColor { get; set; }
    public Color UnderShirtColor { get; set; }
    public Color PantsColor { get; set; }
    public Color ShoeColor { get; set; }
    public TerrariaItemInfo[] Armor { get; set; } = new TerrariaItemInfo[20];
    public TerrariaItemInfo[] Dye { get; set; } = new TerrariaItemInfo[10];
    public TerrariaItemInfo[] Inventory { get; set; } = new TerrariaItemInfo[59];
    public TerrariaItemInfo[] MiscEquips { get; set; } = new TerrariaItemInfo[5];
    public TerrariaItemInfo[] MiscDyes { get; set; } = new TerrariaItemInfo[5];
    public ChestInfo PiggyBank { get; set; } = new ChestInfo(-2);
    public ChestInfo SafeBank { get; set; } = new ChestInfo(-3);
    public ChestInfo DefendersForgeBank { get; set; } = new ChestInfo(-4);
    public ChestInfo VoidVaultBank { get; set; } = new ChestInfo(-5);
    public bool[] VoidVaultInfo { get; set; } = new bool[8];
    public TerrariaBuffTypes[] BuffType { get; set; } = new TerrariaBuffTypes[44];
    public int[] BuffTime { get; set; } = new int[44];
    public int[] SpX { get; set; } = new int[200];
    public int[] SpY { get; set; } = new int[200];
    public string[] SpN { get; set; } = new string[200];
    public int[] SpI { get; set; } = new int[200];
    public bool HotBarLocked { get; set; }
    public bool[] HideInfo { get; set; } = new bool[13];
    public int AnglerQuestsFinished { get; set; }
    public int[] DpadRadialBindings { get; set; } = new int[4];
    public int[] BuilderAccessoryStatus { get; set; } = new int[12];
    public int BartenderQuestLog { get; set; }
    public bool IsDead { get; set; }
    public int RespawnTimer { get; set; }
    public long LastTimePlayerWasSaved { get; set; }
    public int GolferScoreAccumulated { get; set; }
    public TerrariaCreativeUnlocksInfo CreativeUnlocksInfo { get; set; } = new();
    public TerrariaItemInfo[] TemporaryItemSlots { get; set; } = new TerrariaItemInfo[4];
    public CreativePowerInfo CreativePowers { get; set; } = new();
    public bool UnlockedSuperCart { get; set; }
    public bool EnabledSuperCart { get; set; }
    public int CurrentLoadoutIndex { get; set; }
    public TerrariaPlayerEquipmentLoadout[] EquipmentLoadouts { get; set; } = new TerrariaPlayerEquipmentLoadout[3];
    public int VoiceVariant { get; set; }
    public float VoicePitchOffset { get; set; }
    public TerrariaItemInfo[] PendingRefunds { get; set; }
    public HashSet<string> OneTimeDialoguesSeen { get; set; } = [];

    public TerrariaPlayerInfo()
    {
        Armor.FillArray();
        Inventory.FillArray();
        Dye.FillArray();
        MiscEquips.FillArray();
        MiscDyes.FillArray();
        TemporaryItemSlots.FillArray();
        EquipmentLoadouts.FillArray();
    }

    public bool HasItemInAnyInventory(TerrariaItems item)
    {
        return Inventory.Any(x => x.ItemId == item)
            || Armor.Any(x => x.ItemId == item)
            || Dye.Any(x => x.ItemId == item)
            || MiscEquips.Any(x => x.ItemId == item)
            || MiscDyes.Any(x => x.ItemId == item)
            || PiggyBank.Items.Any(x => x.ItemId == item)
            || SafeBank.Items.Any(x => x.ItemId == item)
            || DefendersForgeBank.Items.Any(x => x.ItemId == item)
            || VoidVaultBank.Items.Any(x => x.ItemId == item);
    }
}
