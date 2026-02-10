using TerrariaParsers.Common;
using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Player;

public class TerrariaPlayerEquipmentLoadout
{
    public TerrariaItemInfo[] Armor { get; set; } = new TerrariaItemInfo[20];
    public TerrariaItemInfo[] Dye { get; set; } = new TerrariaItemInfo[10];
    public bool[] HideState { get; set; } = new bool[10];

    public TerrariaPlayerEquipmentLoadout()
    {
        Armor.FillArray();
        Dye.FillArray();
    }
}
