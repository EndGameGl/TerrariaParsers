using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Common;

public static class BinaryReaderExtensions
{
    public static void ReadBitsFromByte(
        this BinaryReader reader,
        bool[] target,
        int startIndex = 0,
        int readAmount = 8
    )
    {
        var data = reader.ReadByte();
        for (int i = 0; i < readAmount; i++)
        {
            target[i + startIndex] = (data & (1 << i)) != 0;
        }
    }

    public static Color ReadColor(this BinaryReader reader)
    {
        return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
    }
}
