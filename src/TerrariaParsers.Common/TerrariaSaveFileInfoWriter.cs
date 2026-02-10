using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Common;

public readonly ref struct TerrariaSaveFileInfoWriter
{
    private readonly BinaryWriter _writer;

    public TerrariaSaveFileInfoWriter(BinaryWriter writer)
    {
        _writer = writer;
    }

    public void Write(TerrariaSaveFileInfo info)
    {
        _writer.Write(0x6369676F6C6572L | ((ulong)info.FileType << 56));
        _writer.Write(info.Revision);
        _writer.Write((ulong)((info.IsFavorite.ToInt() & 1) | 0));
    }
}
