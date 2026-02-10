using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Player;

public class TerrariaCreativeUnlocksInfo
{
    public static Dictionary<TerrariaItems, TerrariaItems> CreativeResearchItemPersistentIdOverride { get; } = [];

    static TerrariaCreativeUnlocksInfo()
    {
        AddItemResearchOverrides(4131, 5325);
        AddItemResearchOverrides(5324, 5329, 5330);
        AddItemResearchOverrides(5437, 5358, 5359, 5360, 5361);
        AddItemResearchOverrides(4346, 5391);
        AddItemResearchOverrides(4767, 5453);
        AddItemResearchOverrides(5309, 5454);
        AddItemResearchOverrides(5323, 5455);
        AddItemResearchOverrides(5526, 2611);
    }

    private static void AddItemResearchOverrides(int itemTypeToUnlock, params int[] itemsThatWillResearchTheItemToUnlock)
    {
        for (int i = 0; i < itemsThatWillResearchTheItemToUnlock.Length; i++)
        {
            AddItemResearchOverride(itemsThatWillResearchTheItemToUnlock[i], itemTypeToUnlock);
        }
    }

    private static void AddItemResearchOverride(int itemTypeToSacrifice, int itemTypeToUnlock)
    {
        CreativeResearchItemPersistentIdOverride[(TerrariaItems)itemTypeToSacrifice] = (TerrariaItems)itemTypeToUnlock;
    }

    public Dictionary<string, int> SacrificeCountByItemPersistentId { get; set; } = [];
}
