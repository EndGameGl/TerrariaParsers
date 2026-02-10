namespace TerrariaParsers.Common.Models;

public class TerrariaSaveFileInfo
{
    public TerrariaFileType FileType { get; set; }
    public uint Revision { get; set; }
    public bool IsFavorite { get; set; }
}
