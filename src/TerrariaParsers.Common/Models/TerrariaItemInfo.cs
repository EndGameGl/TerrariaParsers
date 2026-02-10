using System.Diagnostics;

namespace TerrariaParsers.Common.Models;

[DebuggerDisplay("{ItemId} ({Prefix})")]
public class TerrariaItemInfo
{
    public static Dictionary<TerrariaItems, string> ItemPersistentIdsByNetIds { get; } = [];
    public static Dictionary<string, TerrariaItems> ItemNetIdsByPersistentIds { get; } = [];

    static TerrariaItemInfo()
    {
        for (int k = 0; k < TotalItemCount; k++)
        {
            var itemId = (TerrariaItems)k;
            var name = Enum.GetName(itemId)!; // Item names match enum values, originally it should match class fields in ItemID class
            ItemPersistentIdsByNetIds[itemId] = name;
            ItemNetIdsByPersistentIds[name] = itemId;
        }
    }

    public const int TotalItemCount = 6145;

    public TerrariaItems ItemId { get; set; }
    public ItemPrefixType Prefix { get; set; }
    public int Stack { get; set; }
    public bool IsFavorite { get; set; }

    public void SetItemId(TerrariaItems itemId)
    {
        ItemId = itemId;
    }

    public void SetPrefix(ItemPrefixType prefix)
    {
        Prefix = prefix;
    }

    public TerrariaItemInfo Clone() => (TerrariaItemInfo)MemberwiseClone();
}
