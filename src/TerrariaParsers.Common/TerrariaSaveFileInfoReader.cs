using System.Text;
using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Common;

public readonly ref struct TerrariaSaveFileInfoReader
{
    private readonly BinaryReader _reader;

    public TerrariaSaveFileInfoReader(BinaryReader reader)
    {
        _reader = reader;
    }

    public TerrariaSaveFileInfo ReadInfo(TerrariaFileType expectedType)
    {
        var fileType = ReadFileType(expectedType);

        var revision = _reader.ReadUInt32();

        var isFavorite = (_reader.ReadUInt64() & 1) == 1;

        return new TerrariaSaveFileInfo()
        {
            FileType = fileType,
            Revision = revision,
            IsFavorite = isFavorite,
        };
    }

    private TerrariaFileType ReadFileType(TerrariaFileType expectedType)
    {
        var fileTypeData = _reader.ReadUInt64();
        var fileTypeRaw = BitConverter.GetBytes(fileTypeData)[7];

        if (fileTypeRaw == 0 || fileTypeRaw > 3)
            throw new Exception($"File type: {fileTypeRaw} is out of valid range");

        var fileType = (TerrariaFileType)fileTypeRaw;

        if (fileType != expectedType)
            throw new Exception(
                $"File type: {fileTypeRaw} doesn't match expected type {expectedType}"
            );

        return fileType;
    }
}
