using System;
using System.Security.Cryptography;
using System.Text;
using TerrariaParsers.Common;
using TerrariaParsers.Common.Content;
using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Player;

public readonly ref struct TerrariaPlayerInfoReader
{
    private static readonly HashSet<int> FemaleHairTypes = [5, 6, 9, 11];
    private static readonly byte[] EncryptionKey = new UnicodeEncoding().GetBytes("h3y_gUyZ");

    private readonly Stream _dataStream;

    public TerrariaPlayerInfoReader(Stream dataStream)
    {
        _dataStream = dataStream;
    }

    public TerrariaPlayerInfo ReadInfo()
    {
        var playerInfo = new TerrariaPlayerInfo();

        var eas = Aes.Create();
        eas.Padding = PaddingMode.Zeros;
        eas.BlockSize = 128;

        using var input = new CryptoStream(_dataStream, eas.CreateDecryptor(EncryptionKey, EncryptionKey), CryptoStreamMode.Read);

        using var reader = new BinaryReader(input);

        var fileReleaseVersion = reader.ReadInt32();

        if (fileReleaseVersion >= 135)
        {
            var fileParser = new TerrariaSaveFileInfoReader(reader);
            playerInfo.FileInfo = fileParser.ReadInfo(expectedType: TerrariaFileType.Player);
        }
        else
        {
            // default settings
            playerInfo.FileInfo = new TerrariaSaveFileInfo()
            {
                FileType = TerrariaFileType.Player,
                Revision = 0u,
                IsFavorite = false,
            };
        }

        if (fileReleaseVersion > 317)
        {
            playerInfo.Name = reader.ReadString();
            // Save file is of later version that could be read
            return playerInfo;
        }

        playerInfo.Name = reader.ReadString();
        playerInfo.Difficulty = ReadDifficulty(reader, fileReleaseVersion);
        playerInfo.PlayTime = ReadPlayTime(reader, fileReleaseVersion);
        playerInfo.Hair = ReadHair(reader);
        playerInfo.HairDye = ReadHairDye(reader, fileReleaseVersion);
        playerInfo.Team = ReadTeam(reader, fileReleaseVersion);
        ReadAccessoryVisibility(reader, fileReleaseVersion, playerInfo.HideVisibleAccessory);
        ReadMiscVisibility(reader, fileReleaseVersion, playerInfo.HideMisc);
        ReadSkinVariant(reader, fileReleaseVersion, playerInfo);
        if (fileReleaseVersion < 161 && playerInfo.SkinVariant == PlayerSkinVariant.FemaleCoat)
            playerInfo.SkinVariant = PlayerSkinVariant.FemaleDress;
        playerInfo.StatLife = reader.ReadInt32();
        playerInfo.StatLifeMax = reader.ReadInt32();
        playerInfo.StatLifeMax = Math.Min(playerInfo.StatLifeMax, 500);
        playerInfo.StatMana = reader.ReadInt32();
        playerInfo.StatManaMax = reader.ReadInt32();
        playerInfo.StatManaMax = Math.Min(playerInfo.StatManaMax, 200);
        playerInfo.StatMana = Math.Min(playerInfo.StatMana, 400);
        if (fileReleaseVersion >= 125)
            playerInfo.ExtraAccessory = reader.ReadBoolean();
        if (fileReleaseVersion >= 229)
        {
            playerInfo.UnlockedBiomeTorches = reader.ReadBoolean();
            playerInfo.UsingBiomeTorches = reader.ReadBoolean();
            if (fileReleaseVersion >= 256)
                playerInfo.AteArtisanBread = reader.ReadBoolean();
            if (fileReleaseVersion >= 260)
            {
                playerInfo.UsedAegisCrystal = reader.ReadBoolean();
                playerInfo.UsedAegisFruit = reader.ReadBoolean();
                playerInfo.UsedArcaneCrystal = reader.ReadBoolean();
                playerInfo.UsedGalaxyPearl = reader.ReadBoolean();
                playerInfo.UsedGummyWorm = reader.ReadBoolean();
                playerInfo.UsedAmbrosia = reader.ReadBoolean();
            }
        }
        if (fileReleaseVersion >= 182)
            playerInfo.DungeonDefendersEventCompleted = reader.ReadBoolean();
        if (fileReleaseVersion >= 128)
            playerInfo.TaxMoney = reader.ReadInt32();
        if (fileReleaseVersion >= 254)
        {
            playerInfo.NumberOfDeathsPVE = reader.ReadInt32();
            playerInfo.NumberOfDeathsPVP = reader.ReadInt32();
        }
        playerInfo.HairColor = reader.ReadColor();
        playerInfo.SkinColor = reader.ReadColor();
        playerInfo.EyeColor = reader.ReadColor();
        playerInfo.ShirtColor = reader.ReadColor();
        playerInfo.UnderShirtColor = reader.ReadColor();
        playerInfo.PantsColor = reader.ReadColor();
        playerInfo.ShoeColor = reader.ReadColor();
        ReadInventory(reader, fileReleaseVersion, playerInfo);
        ReadBuffs(reader, fileReleaseVersion, playerInfo);
        ReadSpawnInfo(reader, fileReleaseVersion, playerInfo);
        if (fileReleaseVersion >= 16)
            playerInfo.HotBarLocked = reader.ReadBoolean();
        ReadInfoVisibility(reader, fileReleaseVersion, playerInfo);
        if (fileReleaseVersion >= 98)
            playerInfo.AnglerQuestsFinished = reader.ReadInt32();
        ReadDpadRadialBindings(reader, fileReleaseVersion, playerInfo);
        ReadBuilderAccessoryStatuses(reader, fileReleaseVersion, playerInfo);
        if (fileReleaseVersion >= 181)
            playerInfo.BartenderQuestLog = reader.ReadInt32();
        ReadDeadState(reader, fileReleaseVersion, playerInfo);
        if (fileReleaseVersion >= 202)
            playerInfo.LastTimePlayerWasSaved = reader.ReadInt64();
        if (fileReleaseVersion >= 206)
            playerInfo.GolferScoreAccumulated = reader.ReadInt32();
        ReadCreativeUnlocks(reader, fileReleaseVersion, playerInfo);
        ReadTemporaryItemSlots(reader, fileReleaseVersion, playerInfo);
        ReadCreativePowers(reader, fileReleaseVersion, playerInfo);
        ReadSuperCartInfo(reader, fileReleaseVersion, playerInfo);
        ReadLoadouts(reader, fileReleaseVersion, playerInfo);
        ReadVoiceInfo(reader, fileReleaseVersion, playerInfo);
        ReadPendingRefunds(reader, fileReleaseVersion, playerInfo);
        ReadOneTimeDialoguesSeen(reader, fileReleaseVersion, playerInfo);

        return playerInfo;
    }

    private PlayerCharacterMode ReadDifficulty(BinaryReader reader, int releaseVersion)
    {
        if (releaseVersion < 10)
            return PlayerCharacterMode.Softcore;

        if (releaseVersion >= 17)
            return (PlayerCharacterMode)reader.ReadByte();

        if (reader.ReadBoolean())
            return PlayerCharacterMode.Hardcore;

        return PlayerCharacterMode.Softcore;
    }

    private TimeSpan ReadPlayTime(BinaryReader reader, int releaseVersion)
    {
        if (releaseVersion >= 138)
            return new TimeSpan(reader.ReadInt64());

        return TimeSpan.Zero;
    }

    private int ReadHair(BinaryReader reader)
    {
        var hair = reader.ReadInt32();
        if (hair >= 228)
            return 0;
        return hair;
    }

    private byte ReadHairDye(BinaryReader reader, int releaseVersion)
    {
        if (releaseVersion >= 82)
            return reader.ReadByte();
        return 0;
    }

    private byte ReadTeam(BinaryReader reader, int releaseVersion)
    {
        if (releaseVersion >= 283)
        {
            return reader.ReadByte();
        }
        return 0;
    }

    private void ReadAccessoryVisibility(BinaryReader reader, int releaseVersion, bool[] visibilityData)
    {
        if (releaseVersion >= 83)
        {
            reader.ReadBitsFromByte(visibilityData, startIndex: 0, readAmount: 8);
        }

        if (releaseVersion >= 124)
        {
            reader.ReadBitsFromByte(visibilityData, startIndex: 8, readAmount: 2);
        }
    }

    private void ReadMiscVisibility(BinaryReader reader, int releaseVersion, bool[] visibilityData)
    {
        if (releaseVersion < 119)
            return;

        reader.ReadBitsFromByte(visibilityData);
    }

    private void ReadSkinVariant(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion <= 17)
        {
            playerInfo.Male = !FemaleHairTypes.Contains(playerInfo.Hair);
            return;
        }

        if (releaseVersion < 107)
        {
            playerInfo.Male = reader.ReadBoolean();
            return;
        }

        playerInfo.SkinVariant = (PlayerSkinVariant)reader.ReadByte();
    }

    private void ReadInventory(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion >= 38)
        {
            ReadArmor(reader, releaseVersion, playerInfo);
            ReadDyes(reader, releaseVersion, playerInfo);
            ReadInventoryItems(reader, releaseVersion, playerInfo);
            ReadMiscItems(reader, releaseVersion, playerInfo);
            ReadPiggyBankAndSafe(reader, releaseVersion, playerInfo);
            ReadDefendersForge(reader, releaseVersion, playerInfo);
            ReadVoidVault(reader, releaseVersion, playerInfo);
            if (releaseVersion >= 199)
                reader.ReadBitsFromByte(playerInfo.VoidVaultInfo);
        }
        else
        {
            ReadLegacyInventory(reader, releaseVersion, playerInfo);
        }

        if (releaseVersion < 58)
        {
            for (var i = 40; i < 48; i++)
            {
                playerInfo.Inventory[i + 10] = playerInfo.Inventory[i].Clone();
                playerInfo.Inventory[i].SetItemId(TerrariaItems.None);
            }
        }
    }

    private void ReadArmor(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 124)
        {
            var count = releaseVersion >= 81 ? 16 : 11;
            for (int i = 0; i < count; i++)
            {
                var idx = i;

                if (idx >= 8)
                    idx += 2;

                playerInfo.Armor[idx].SetItemId((TerrariaItems)reader.ReadInt32());
                playerInfo.Armor[idx].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
        }
        else
        {
            for (int i = 0; i < 20; i++)
            {
                playerInfo.Armor[i].SetItemId((TerrariaItems)reader.ReadInt32());
                playerInfo.Armor[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
        }
    }

    private void ReadDyes(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 47)
            return;

        var amount = releaseVersion switch
        {
            >= 124 => 10,
            >= 81 => 8,
            _ => 3,
        };

        for (int i = 0; i < amount; i++)
        {
            playerInfo.Dye[i].SetItemId((TerrariaItems)reader.ReadInt32());
            playerInfo.Dye[i].SetPrefix((ItemPrefixType)reader.ReadByte());
        }
    }

    private void ReadInventoryItems(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion >= 58)
        {
            for (int i = 0; i < 58; i++)
            {
                int itemId = reader.ReadInt32();
                // if item id is larger than item amount
                if (itemId >= TerrariaItemInfo.TotalItemCount)
                {
                    playerInfo.Inventory[i].SetItemId(TerrariaItems.None);
                    reader.ReadInt32();
                    reader.ReadByte();
                    if (releaseVersion >= 114)
                    {
                        reader.ReadBoolean();
                    }
                }
                else
                {
                    playerInfo.Inventory[i].SetItemId((TerrariaItems)itemId);
                    playerInfo.Inventory[i].Stack = reader.ReadInt32();
                    playerInfo.Inventory[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                    if (releaseVersion >= 114)
                    {
                        playerInfo.Inventory[i].IsFavorite = reader.ReadBoolean();
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 48; i++)
            {
                int itemId = reader.ReadInt32();
                // if item id is larger than item amount
                if (itemId >= TerrariaItemInfo.TotalItemCount)
                {
                    playerInfo.Inventory[i].SetItemId(TerrariaItems.None);
                    reader.ReadInt32();
                    reader.ReadByte();
                }
                else
                {
                    playerInfo.Inventory[i].SetItemId((TerrariaItems)itemId);
                    playerInfo.Inventory[i].Stack = reader.ReadInt32();
                    playerInfo.Inventory[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                }
            }
        }
    }

    private void ReadMiscItems(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 117)
            return;
        if (releaseVersion < 136)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i != 1)
                {
                    int itemId = reader.ReadInt32();
                    if (itemId >= TerrariaItemInfo.TotalItemCount)
                    {
                        playerInfo.MiscEquips[i].SetItemId(TerrariaItems.None);
                        reader.ReadByte();
                    }
                    else
                    {
                        playerInfo.MiscEquips[i].SetItemId((TerrariaItems)itemId);
                        playerInfo.MiscEquips[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                    }
                    itemId = reader.ReadInt32();
                    if (itemId >= TerrariaItemInfo.TotalItemCount)
                    {
                        playerInfo.MiscDyes[i].SetItemId(TerrariaItems.None);
                        reader.ReadByte();
                    }
                    else
                    {
                        playerInfo.MiscDyes[i].SetItemId((TerrariaItems)itemId);
                        playerInfo.MiscDyes[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                int itemId = reader.ReadInt32();
                if (itemId >= TerrariaItemInfo.TotalItemCount)
                {
                    playerInfo.MiscEquips[i].SetItemId(TerrariaItems.None);
                    reader.ReadByte();
                }
                else
                {
                    playerInfo.MiscEquips[i].SetItemId((TerrariaItems)itemId);
                    playerInfo.MiscEquips[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                }
                itemId = reader.ReadInt32();
                if (itemId >= TerrariaItemInfo.TotalItemCount)
                {
                    playerInfo.MiscDyes[i].SetItemId(TerrariaItems.None);
                    reader.ReadByte();
                }
                else
                {
                    playerInfo.MiscDyes[i].SetItemId((TerrariaItems)itemId);
                    playerInfo.MiscDyes[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                }
            }
        }
    }

    private void ReadPiggyBankAndSafe(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion >= 58)
        {
            for (var i = 0; i < 40; i++)
            {
                playerInfo.PiggyBank.Items[i].SetItemId((TerrariaItems)reader.ReadInt32());
                playerInfo.PiggyBank.Items[i].Stack = reader.ReadInt32();
                playerInfo.PiggyBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
            for (var i = 0; i < 40; i++)
            {
                playerInfo.SafeBank.Items[i].SetItemId((TerrariaItems)reader.ReadInt32());
                playerInfo.SafeBank.Items[i].Stack = reader.ReadInt32();
                playerInfo.SafeBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
        }
        else
        {
            for (var i = 0; i < 20; i++)
            {
                playerInfo.PiggyBank.Items[i].SetItemId((TerrariaItems)reader.ReadInt32());
                playerInfo.PiggyBank.Items[i].Stack = reader.ReadInt32();
                playerInfo.PiggyBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
            for (var i = 0; i < 20; i++)
            {
                playerInfo.SafeBank.Items[i].SetItemId((TerrariaItems)reader.ReadInt32());
                playerInfo.SafeBank.Items[i].Stack = reader.ReadInt32();
                playerInfo.SafeBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
        }
    }

    private void ReadDefendersForge(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 182)
            return;

        for (var i = 0; i < 40; i++)
        {
            playerInfo.DefendersForgeBank.Items[i].SetItemId((TerrariaItems)reader.ReadInt32());
            playerInfo.DefendersForgeBank.Items[i].Stack = reader.ReadInt32();
            playerInfo.DefendersForgeBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());
        }
    }

    private void ReadVoidVault(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 198)
            return;

        for (var i = 0; i < 40; i++)
        {
            playerInfo.VoidVaultBank.Items[i].SetItemId((TerrariaItems)reader.ReadInt32());
            playerInfo.VoidVaultBank.Items[i].Stack = reader.ReadInt32();
            playerInfo.VoidVaultBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());

            if (releaseVersion >= 255)
                playerInfo.VoidVaultBank.Items[i].IsFavorite = reader.ReadBoolean();
        }
    }

    private void ReadLegacyInventory(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        for (int i = 0; i < 8; i++)
        {
            playerInfo.Armor[i].SetItemId(TerrariaLegacyItemConverter.FromLegacyName(reader.ReadString(), releaseVersion));
            if (releaseVersion >= 36)
            {
                playerInfo.Armor[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
        }
        if (releaseVersion >= 6)
        {
            for (int i = 10; i < 13; i++)
            {
                playerInfo.Armor[i].SetItemId(TerrariaLegacyItemConverter.FromLegacyName(reader.ReadString(), releaseVersion));
                if (releaseVersion >= 36)
                {
                    playerInfo.Armor[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                }
            }
        }
        for (int i = 0; i < 44; i++)
        {
            playerInfo.Inventory[i].SetItemId(TerrariaLegacyItemConverter.FromLegacyName(reader.ReadString(), releaseVersion));
            playerInfo.Inventory[i].Stack = reader.ReadInt32();
            if (releaseVersion >= 36)
            {
                playerInfo.Inventory[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
        }
        if (releaseVersion >= 15)
        {
            for (int i = 44; i < 48; i++)
            {
                playerInfo.Inventory[i].SetItemId(TerrariaLegacyItemConverter.FromLegacyName(reader.ReadString(), releaseVersion));
                playerInfo.Inventory[i].Stack = reader.ReadInt32();
                if (releaseVersion >= 36)
                {
                    playerInfo.Inventory[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                }
            }
        }
        for (int i = 0; i < 20; i++)
        {
            playerInfo.PiggyBank.Items[i].SetItemId(TerrariaLegacyItemConverter.FromLegacyName(reader.ReadString(), releaseVersion));
            playerInfo.PiggyBank.Items[i].Stack = reader.ReadInt32();
            if (releaseVersion >= 36)
            {
                playerInfo.PiggyBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());
            }
        }
        if (releaseVersion >= 20)
        {
            for (int i = 0; i < 20; i++)
            {
                playerInfo.SafeBank.Items[i].SetItemId(TerrariaLegacyItemConverter.FromLegacyName(reader.ReadString(), releaseVersion));
                playerInfo.SafeBank.Items[i].Stack = reader.ReadInt32();
                if (releaseVersion >= 36)
                {
                    playerInfo.SafeBank.Items[i].SetPrefix((ItemPrefixType)reader.ReadByte());
                }
            }
        }
    }

    private void ReadBuffs(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 11)
            return;

        int maxBuffAmount = 22;
        if (releaseVersion < 74)
        {
            maxBuffAmount = 10;
        }
        if (releaseVersion >= 252)
        {
            maxBuffAmount = 44;
        }
        for (int i = 0; i < maxBuffAmount; i++)
        {
            playerInfo.BuffType[i] = (TerrariaBuffTypes)reader.ReadInt32();
            playerInfo.BuffTime[i] = reader.ReadInt32();
            if (playerInfo.BuffType[i] == 0)
            {
                i--;
                maxBuffAmount--;
            }
        }
    }

    private void ReadSpawnInfo(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        for (int i = 0; i < 200; i++)
        {
            int spX = reader.ReadInt32();
            if (spX == -1)
            {
                break;
            }
            playerInfo.SpX[i] = spX;
            playerInfo.SpY[i] = reader.ReadInt32();
            playerInfo.SpI[i] = reader.ReadInt32();
            playerInfo.SpN[i] = reader.ReadString();
        }
    }

    private void ReadInfoVisibility(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 115)
            return;

        for (int i = 0; i < 13; i++)
        {
            playerInfo.HideInfo[i] = reader.ReadBoolean();
        }
    }

    private void ReadDpadRadialBindings(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 162)
            return;

        for (int i = 0; i < 4; i++)
        {
            playerInfo.DpadRadialBindings[i] = reader.ReadInt32();
        }
    }

    private void ReadBuilderAccessoryStatuses(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 164)
            return;

        int builderAccessoryNumber = 8;
        if (releaseVersion >= 167)
            builderAccessoryNumber = 10;

        if (releaseVersion >= 197)
            builderAccessoryNumber = 11;

        if (releaseVersion >= 230)
            builderAccessoryNumber = 12;

        for (int i = 0; i < builderAccessoryNumber; i++)
            playerInfo.BuilderAccessoryStatus[i] = reader.ReadInt32();

        if (releaseVersion < 210)
            playerInfo.BuilderAccessoryStatus[(int)BuilderAccessoryToggles.RulerLine] = 1;

        if (releaseVersion < 249)
        {
            bool hasGrandDesign = false;
            for (int i = 0; i < 58; i++)
            {
                if (playerInfo.Inventory[i].ItemId == TerrariaItems.WireKite)
                {
                    hasGrandDesign = true;
                    break;
                }
            }
            if (hasGrandDesign)
            {
                playerInfo.BuilderAccessoryStatus[(int)BuilderAccessoryToggles.RulerGrid] = 1;
            }
        }
    }

    private void ReadDeadState(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 200)
            return;

        playerInfo.IsDead = reader.ReadBoolean();
        if (playerInfo.IsDead)
        {
            playerInfo.RespawnTimer = Math.Clamp(reader.ReadInt32(), 0, 60000);
        }
    }

    private void ReadCreativeUnlocks(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 218)
            return;

        if (releaseVersion >= 282)
            reader.ReadBoolean();

        var entries = reader.ReadInt32();
        for (int i = 0; i < entries; i++)
        {
            string key = reader.ReadString();
            int value = reader.ReadInt32();

            if (TerrariaItemInfo.ItemNetIdsByPersistentIds.TryGetValue(key, out var itemId))
            {
                // some items unlock same thing
                if (TerrariaCreativeUnlocksInfo.CreativeResearchItemPersistentIdOverride.TryGetValue(itemId, out var overridenItemId))
                {
                    itemId = overridenItemId;
                }
                //overwrite key in case item id was overriden
                if (TerrariaItemInfo.ItemPersistentIdsByNetIds.TryGetValue(itemId, out var persistentKey))
                {
                    key = persistentKey;
                }
            }

            playerInfo.CreativeUnlocksInfo.SacrificeCountByItemPersistentId[key] = value;
        }

        var test = TerrariaSacrificesData.SacrificesData;
    }

    private void ReadTemporaryItemSlots(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        var tempSlots = new bool[8];
        reader.ReadBitsFromByte(tempSlots);

        TryReadTemporaryItemSlot(reader, releaseVersion, playerInfo, tempSlots, 0); // mouse item
        TryReadTemporaryItemSlot(reader, releaseVersion, playerInfo, tempSlots, 1); // creative sacrifice
        TryReadTemporaryItemSlot(reader, releaseVersion, playerInfo, tempSlots, 2); // guide item
        TryReadTemporaryItemSlot(reader, releaseVersion, playerInfo, tempSlots, 3); // tinkerer item
    }

    private void TryReadTemporaryItemSlot(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo, bool[] slotsInfo, int index)
    {
        if (slotsInfo[index])
        {
            ReadGenericItem(reader, releaseVersion, playerInfo.TemporaryItemSlots[index]);
        }
    }

    private void ReadCreativePowers(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 220)
            return;

        while (reader.ReadBoolean())
        {
            var powerId = reader.ReadUInt16();

            // powers typically range from 0 to 14, so out of that range means we finished reading
            if (powerId < 0 || powerId > 14)
                break;

            if (powerId == 5) // godmode
            {
                playerInfo.CreativePowers.GodmodePowerEnabled = reader.ReadBoolean();
                continue;
            }

            if (powerId == 11) // far placement
            {
                playerInfo.CreativePowers.FarPlacementRangePowerEnabled = reader.ReadBoolean();
                continue;
            }

            if (powerId == 14) // spawn rate
            {
                playerInfo.CreativePowers.SpawnRatePowerSliderValue = reader.ReadSingle();
                continue;
            }
        }

        if (playerInfo.Difficulty != PlayerCharacterMode.Creative)
            playerInfo.CreativePowers = new TerrariaPlayerInfo.CreativePowerInfo(); // resetting values if not journey mode character
    }

    private void ReadSuperCartInfo(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion >= 253)
        {
            var bits = new bool[2];
            reader.ReadBitsFromByte(bits, readAmount: 2);
            playerInfo.UnlockedSuperCart = bits[0];
            playerInfo.EnabledSuperCart = bits[1];
        }
        else
        {
            playerInfo.UnlockedSuperCart = playerInfo.HasItemInAnyInventory(TerrariaItems.MinecartMech);
        }
    }

    private void ReadLoadouts(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 262)
            return;

        int value = reader.ReadInt32();
        playerInfo.CurrentLoadoutIndex = Math.Clamp(value, 0, playerInfo.EquipmentLoadouts.Length - 1);

        for (var i = 0; i < playerInfo.EquipmentLoadouts.Length; i++)
        {
            var loadout = playerInfo.EquipmentLoadouts[i];
            for (var j = 0; j < loadout.Armor.Length; j++)
            {
                ReadGenericItem(reader, releaseVersion, loadout.Armor[j]);
            }
            for (var j = 0; j < loadout.Dye.Length; j++)
            {
                ReadGenericItem(reader, releaseVersion, loadout.Dye[j]);
            }
            for (int j = 0; j < loadout.HideState.Length; j++)
            {
                loadout.HideState[j] = reader.ReadBoolean();
            }
        }
    }

    private void ReadGenericItem(BinaryReader reader, int releaseVersion, TerrariaItemInfo item)
    {
        var itemId = reader.ReadInt32();
        // invalid item
        if (itemId > TerrariaItemInfo.TotalItemCount)
        {
            reader.ReadInt32();
            reader.ReadByte();
            item.SetItemId(TerrariaItems.None);
        }
        else
        {
            item.SetItemId((TerrariaItems)itemId);
            item.Stack = reader.ReadInt32();
            item.SetPrefix((ItemPrefixType)reader.ReadByte());
        }
    }

    private void ReadVoiceInfo(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion >= 280)
        {
            playerInfo.VoiceVariant = reader.ReadByte();
        }
        else
        {
            playerInfo.SkinVariant = (PlayerSkinVariant)Math.Clamp((int)playerInfo.SkinVariant, 0, 11);
            playerInfo.VoiceVariant = (playerInfo.Male ? 1 : 2);
            playerInfo.VoicePitchOffset = 0f;
        }
        if (releaseVersion >= 281)
        {
            playerInfo.VoicePitchOffset = reader.ReadSingle();
        }
        else
        {
            playerInfo.VoicePitchOffset = 0f;
        }
    }

    private void ReadPendingRefunds(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 300)
            return;

        playerInfo.PendingRefunds = new TerrariaItemInfo[reader.ReadInt32()];
        for (var i = 0; i < playerInfo.PendingRefunds.Length; i++)
        {
            playerInfo.PendingRefunds[i] = new TerrariaItemInfo();
            ReadGenericItem(reader, releaseVersion, playerInfo.PendingRefunds[i]);
        }
    }

    private void ReadOneTimeDialoguesSeen(BinaryReader reader, int releaseVersion, TerrariaPlayerInfo playerInfo)
    {
        if (releaseVersion < 310)
            return;

        int amount = reader.ReadInt32();
        for (var i = 0; i < amount; i++)
        {
            playerInfo.OneTimeDialoguesSeen.Add(reader.ReadString());
        }
    }
}
