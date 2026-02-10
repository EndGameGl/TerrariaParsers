using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Common;

public static class BinaryWriterExtensions
{
    public static void Write(this BinaryWriter writer, Color color)
    {
        writer.Write(color.R);
        writer.Write(color.G);
        writer.Write(color.B);
    }

    public static void Write(this BinaryWriter writer, TerrariaItems itemId)
    {
        writer.Write((int)itemId);
    }

    public static void Write(this BinaryWriter writer, ItemPrefixType prefixType)
    {
        writer.Write((byte)prefixType);
    }
}
