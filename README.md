# TerrariaParsers

Parser projects for Terraria version 1.4.5.4, written in .NET

## TerrariaParsers.Player
Allows to read, edit and write `.plr` files

To read player file:
```cs
using TerrariaParsers.Player;

using var readStream = File.OpenRead(pathToPlrFile);
var reader = new TerrariaPlayerInfoReader(readStream);
TerrariaPlayerInfo playerInfo = reader.ReadInfo();
```

To write player file:
```cs
using TerrariaParsers.Player;

using var fileWriteStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
var writer = new TerrariaPlayerInfoWriter(fileWriteStream);
writer.WritePlayerInfo(playerInfo);
```
