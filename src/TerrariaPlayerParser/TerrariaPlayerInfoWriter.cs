using System.Security.Cryptography;
using System.Text;
using TerrariaParsers.Common;
using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Player;

public readonly ref struct TerrariaPlayerInfoWriter
{
    private static readonly HashSet<TerrariaBuffTypes> IgnoreSaveBuffs =
    [
        (TerrariaBuffTypes)20,
        (TerrariaBuffTypes)22,
        (TerrariaBuffTypes)23,
        (TerrariaBuffTypes)24,
        (TerrariaBuffTypes)28,
        (TerrariaBuffTypes)30,
        (TerrariaBuffTypes)31,
        (TerrariaBuffTypes)34,
        (TerrariaBuffTypes)35,
        (TerrariaBuffTypes)37,
        (TerrariaBuffTypes)38,
        (TerrariaBuffTypes)39,
        (TerrariaBuffTypes)43,
        (TerrariaBuffTypes)44,
        (TerrariaBuffTypes)46,
        (TerrariaBuffTypes)47,
        (TerrariaBuffTypes)48,
        (TerrariaBuffTypes)58,
        (TerrariaBuffTypes)59,
        (TerrariaBuffTypes)60,
        (TerrariaBuffTypes)62,
        (TerrariaBuffTypes)63,
        (TerrariaBuffTypes)64,
        (TerrariaBuffTypes)67,
        (TerrariaBuffTypes)68,
        (TerrariaBuffTypes)69,
        (TerrariaBuffTypes)70,
        (TerrariaBuffTypes)72,
        (TerrariaBuffTypes)80,
        (TerrariaBuffTypes)87,
        (TerrariaBuffTypes)158,
        (TerrariaBuffTypes)146,
        (TerrariaBuffTypes)147,
        (TerrariaBuffTypes)215,
        (TerrariaBuffTypes)88,
        (TerrariaBuffTypes)89,
        (TerrariaBuffTypes)94,
        (TerrariaBuffTypes)95,
        (TerrariaBuffTypes)96,
        (TerrariaBuffTypes)97,
        (TerrariaBuffTypes)98,
        (TerrariaBuffTypes)99,
        (TerrariaBuffTypes)100,
        (TerrariaBuffTypes)103,
        (TerrariaBuffTypes)119,
        (TerrariaBuffTypes)120,
        (TerrariaBuffTypes)125,
        (TerrariaBuffTypes)126,
        (TerrariaBuffTypes)133,
        (TerrariaBuffTypes)134,
        (TerrariaBuffTypes)135,
        (TerrariaBuffTypes)139,
        (TerrariaBuffTypes)140,
        (TerrariaBuffTypes)137,
        (TerrariaBuffTypes)144,
        (TerrariaBuffTypes)161,
        (TerrariaBuffTypes)163,
        (TerrariaBuffTypes)164,
        (TerrariaBuffTypes)170,
        (TerrariaBuffTypes)171,
        (TerrariaBuffTypes)172,
        (TerrariaBuffTypes)182,
        (TerrariaBuffTypes)187,
        (TerrariaBuffTypes)188,
        (TerrariaBuffTypes)194,
        (TerrariaBuffTypes)195,
        (TerrariaBuffTypes)196,
        (TerrariaBuffTypes)197,
        (TerrariaBuffTypes)198,
        (TerrariaBuffTypes)199,
        (TerrariaBuffTypes)205,
        (TerrariaBuffTypes)213,
        (TerrariaBuffTypes)214,
        (TerrariaBuffTypes)263,
        (TerrariaBuffTypes)271,
        (TerrariaBuffTypes)322,
        (TerrariaBuffTypes)320,
        (TerrariaBuffTypes)321,
        (TerrariaBuffTypes)325,
        (TerrariaBuffTypes)335,
        (TerrariaBuffTypes)150,
        (TerrariaBuffTypes)93,
        (TerrariaBuffTypes)159,
        (TerrariaBuffTypes)29,
        (TerrariaBuffTypes)348,
        (TerrariaBuffTypes)366,
        (TerrariaBuffTypes)353,
        (TerrariaBuffTypes)355,
        (TerrariaBuffTypes)385,
        (TerrariaBuffTypes)386,
    ];

    private static readonly byte[] EncryptionKey = new UnicodeEncoding().GetBytes("h3y_gUyZ");

    private readonly FileStream _fileStream;

    public TerrariaPlayerInfoWriter(FileStream fileStream)
    {
        _fileStream = fileStream;
    }

    public void WritePlayerInfo(TerrariaPlayerInfo playerInfo)
    {
        using var aes = Aes.Create();

        using var cryptoStream = new CryptoStream(_fileStream, aes.CreateEncryptor(EncryptionKey, EncryptionKey), CryptoStreamMode.Write);
        using var binaryWriter = new BinaryWriter(cryptoStream);
        binaryWriter.Write(317);

        var metadataWriter = new TerrariaSaveFileInfoWriter(binaryWriter);
        metadataWriter.Write(playerInfo.FileInfo);

        WritePlayerInfoInternal(binaryWriter, playerInfo);

        binaryWriter.Flush();
        cryptoStream.FlushFinalBlock();
        _fileStream.Flush();
    }

    private void WritePlayerInfoInternal(BinaryWriter writer, TerrariaPlayerInfo playerInfo)
    {
        writer.Write(playerInfo.Name);
        writer.Write((byte)playerInfo.Difficulty);
        writer.Write(playerInfo.PlayTime.Ticks);
        writer.Write(playerInfo.Hair);
        writer.Write(playerInfo.HairDye);
        writer.Write((byte)playerInfo.Team);
        writer.Write(playerInfo.HideVisibleAccessory.BoolArrayToByte(length: 8, start: 0));
        writer.Write(playerInfo.HideVisibleAccessory.BoolArrayToByte(length: 2, start: 8));
        writer.Write(playerInfo.HideMisc.BoolArrayToByte(length: 8, start: 0));
        writer.Write((byte)playerInfo.SkinVariant);
        writer.Write(playerInfo.StatLife);
        writer.Write(playerInfo.StatLifeMax);
        writer.Write(playerInfo.StatMana);
        writer.Write(playerInfo.StatManaMax);
        writer.Write(playerInfo.ExtraAccessory);
        writer.Write(playerInfo.UnlockedBiomeTorches);
        writer.Write(playerInfo.UsingBiomeTorches);
        writer.Write(playerInfo.AteArtisanBread);
        writer.Write(playerInfo.UsedAegisCrystal);
        writer.Write(playerInfo.UsedAegisFruit);
        writer.Write(playerInfo.UsedArcaneCrystal);
        writer.Write(playerInfo.UsedGalaxyPearl);
        writer.Write(playerInfo.UsedGummyWorm);
        writer.Write(playerInfo.UsedAmbrosia);
        writer.Write(playerInfo.DungeonDefendersEventCompleted);
        writer.Write(playerInfo.TaxMoney);
        writer.Write(playerInfo.NumberOfDeathsPVE);
        writer.Write(playerInfo.NumberOfDeathsPVP);
        writer.Write(playerInfo.HairColor);
        writer.Write(playerInfo.SkinColor);
        writer.Write(playerInfo.EyeColor);
        writer.Write(playerInfo.ShirtColor);
        writer.Write(playerInfo.UnderShirtColor);
        writer.Write(playerInfo.PantsColor);
        writer.Write(playerInfo.ShoeColor);
        for (int i = 0; i < playerInfo.Armor.Length; i++)
        {
            writer.Write(playerInfo.Armor[i].ItemId);
            writer.Write(playerInfo.Armor[i].Prefix);
        }
        for (int i = 0; i < playerInfo.Dye.Length; i++)
        {
            writer.Write(playerInfo.Dye[i].ItemId);
            writer.Write(playerInfo.Dye[i].Prefix);
        }
        for (int i = 0; i < 58; i++)
        {
            writer.Write(playerInfo.Inventory[i].ItemId);
            writer.Write(playerInfo.Inventory[i].Stack);
            writer.Write(playerInfo.Inventory[i].Prefix);
            writer.Write(playerInfo.Inventory[i].IsFavorite);
        }
        for (int i = 0; i < playerInfo.MiscEquips.Length; i++)
        {
            writer.Write(playerInfo.MiscEquips[i].ItemId);
            writer.Write(playerInfo.MiscEquips[i].Prefix);
            writer.Write(playerInfo.MiscDyes[i].ItemId);
            writer.Write(playerInfo.MiscDyes[i].Prefix);
        }
        for (int i = 0; i < playerInfo.PiggyBank.Items.Length; i++)
        {
            writer.Write(playerInfo.PiggyBank.Items[i].ItemId);
            writer.Write(playerInfo.PiggyBank.Items[i].Stack);
            writer.Write(playerInfo.PiggyBank.Items[i].Prefix);
        }
        for (int i = 0; i < playerInfo.SafeBank.Items.Length; i++)
        {
            writer.Write(playerInfo.SafeBank.Items[i].ItemId);
            writer.Write(playerInfo.SafeBank.Items[i].Stack);
            writer.Write(playerInfo.SafeBank.Items[i].Prefix);
        }
        for (int i = 0; i < playerInfo.DefendersForgeBank.Items.Length; i++)
        {
            writer.Write(playerInfo.DefendersForgeBank.Items[i].ItemId);
            writer.Write(playerInfo.DefendersForgeBank.Items[i].Stack);
            writer.Write(playerInfo.DefendersForgeBank.Items[i].Prefix);
        }
        for (int i = 0; i < playerInfo.VoidVaultBank.Items.Length; i++)
        {
            writer.Write(playerInfo.VoidVaultBank.Items[i].ItemId);
            writer.Write(playerInfo.VoidVaultBank.Items[i].Stack);
            writer.Write(playerInfo.VoidVaultBank.Items[i].Prefix);
            writer.Write(playerInfo.VoidVaultBank.Items[i].IsFavorite);
        }
        writer.Write(playerInfo.VoidVaultInfo.BoolArrayToByte(length: 8, start: 0));
        for (int i = 0; i < 44; i++) // max 44 buffs
        {
            if (IgnoreSaveBuffs.Contains(playerInfo.BuffType[i]))
            {
                writer.Write(0);
                writer.Write(0);
            }
            else
            {
                writer.Write((int)playerInfo.BuffType[i]);
                writer.Write(playerInfo.BuffTime[i]);
            }
        }
        for (int i = 0; i < 200; i++)
        {
            if (playerInfo.SpN[i] == null)
            {
                writer.Write(-1);
                break;
            }
            writer.Write(playerInfo.SpX[i]);
            writer.Write(playerInfo.SpY[i]);
            writer.Write(playerInfo.SpI[i]);
            writer.Write(playerInfo.SpN[i]);
        }
        writer.Write(playerInfo.HotBarLocked);
        for (int i = 0; i < playerInfo.HideInfo.Length; i++)
            writer.Write(playerInfo.HideInfo[i]);

        writer.Write(playerInfo.AnglerQuestsFinished);
        for (int i = 0; i < playerInfo.DpadRadialBindings.Length; i++)
            writer.Write(playerInfo.DpadRadialBindings[i]);

        for (int i = 0; i < playerInfo.BuilderAccessoryStatus.Length; i++)
            writer.Write(playerInfo.BuilderAccessoryStatus[i]);

        writer.Write(playerInfo.BartenderQuestLog);
        writer.Write(playerInfo.IsDead);
        if (playerInfo.IsDead)
            writer.Write(playerInfo.RespawnTimer);

        writer.Write(DateTime.UtcNow.ToBinary());
        writer.Write(playerInfo.GolferScoreAccumulated);
        writer.Write(false);
        writer.Write(playerInfo.CreativeUnlocksInfo.SacrificeCountByItemPersistentId.Count);
        foreach (var (itemId, count) in playerInfo.CreativeUnlocksInfo.SacrificeCountByItemPersistentId)
        {
            writer.Write(itemId);
            writer.Write(count);
        }
        var tempSlotsState = new bool[playerInfo.TemporaryItemSlots.Length];
        for (int i = 0; i < playerInfo.TemporaryItemSlots.Length; i++)
            tempSlotsState[i] = playerInfo.TemporaryItemSlots[i].ItemId != TerrariaItems.None;

        writer.Write(tempSlotsState.BoolArrayToByte(length: 4, start: 0));
        if (tempSlotsState[0])
            GenericItemWrite(writer, playerInfo.TemporaryItemSlots[0]);
        if (tempSlotsState[1])
            GenericItemWrite(writer, playerInfo.TemporaryItemSlots[1]);
        if (tempSlotsState[2])
            GenericItemWrite(writer, playerInfo.TemporaryItemSlots[2]);
        if (tempSlotsState[3])
            GenericItemWrite(writer, playerInfo.TemporaryItemSlots[3]);

        writer.Write(true);
        writer.Write((ushort)5);
        writer.Write(playerInfo.CreativePowers.GodmodePowerEnabled);

        writer.Write(true);
        writer.Write((ushort)11);
        writer.Write(playerInfo.CreativePowers.FarPlacementRangePowerEnabled);

        writer.Write(true);
        writer.Write((ushort)14);
        writer.Write(playerInfo.CreativePowers.SpawnRatePowerSliderValue);

        writer.Write(false);

        var superCartState = new bool[2];
        superCartState[0] = playerInfo.UnlockedSuperCart;
        superCartState[1] = playerInfo.EnabledSuperCart;
        writer.Write(superCartState.BoolArrayToByte(length: 2, start: 0));

        writer.Write(playerInfo.CurrentLoadoutIndex);

        for (int i = 0; i < playerInfo.EquipmentLoadouts.Length; i++)
        {
            var loadout = playerInfo.EquipmentLoadouts[i];
            for (var j = 0; j < loadout.Armor.Length; j++)
                GenericItemWrite(writer, loadout.Armor[j]);
            for (var j = 0; j < loadout.Dye.Length; j++)
                GenericItemWrite(writer, loadout.Dye[j]);
            for (var j = 0; j < loadout.HideState.Length; j++)
                writer.Write(loadout.HideState[j]);
        }

        writer.Write((byte)playerInfo.VoiceVariant);
        writer.Write(playerInfo.VoicePitchOffset);

        writer.Write(playerInfo.PendingRefunds.Length);
        for (int i = 0; i < playerInfo.PendingRefunds.Length; i++)
            GenericItemWrite(writer, playerInfo.PendingRefunds[i]);

        writer.Write(playerInfo.OneTimeDialoguesSeen.Count);
        foreach (var oneTimeDialogueSeen in playerInfo.OneTimeDialoguesSeen)
            writer.Write(oneTimeDialogueSeen);
    }

    private void GenericItemWrite(BinaryWriter writer, TerrariaItemInfo item)
    {
        writer.Write(item.ItemId);
        writer.Write(item.Stack);
        writer.Write(item.Prefix);
    }
}
