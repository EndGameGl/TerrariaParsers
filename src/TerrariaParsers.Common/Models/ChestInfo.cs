namespace TerrariaParsers.Common.Models;

public class ChestInfo
{
    public bool IsBank { get; set; }
    public int Index { get; set; }
    public TerrariaItemInfo[] Items { get; set; } = new TerrariaItemInfo[40];

    public ChestInfo(int index = 0)
    {
        Index = index;
        Items.FillArray();
    }
}
