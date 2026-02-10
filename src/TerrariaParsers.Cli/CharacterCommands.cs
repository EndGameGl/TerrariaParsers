using System.Diagnostics.CodeAnalysis;
using ConsoleAppFramework;
using TerrariaParsers.Player;

namespace TerrariaParsers.Cli;

[RegisterCommands("char")]
public class CharacterCommands
{
    [Command("update-name")]
    public void UpdateName(string path, string name)
    {
        if (!IsValidFile(path, out var invalidReason))
        {
            Console.WriteLine(invalidReason);
            return;
        }

        if (!TryReadPlayerInfo(path, out var player, out var failReason))
        {
            Console.WriteLine(failReason);
            return;
        }

        player.Name = name;

        if (!TrySavePlayerInfo(path, player, out failReason))
        {
            Console.WriteLine(failReason);
            return;
        }

        Console.WriteLine("Character name was updated");
    }

    private static bool IsValidFile(string path, [NotNullWhen(false)] out string? invalidReason)
    {
        invalidReason = null;

        if (!File.Exists(path))
        {
            invalidReason = "File was not found";
            return false;
        }

        if (!Path.HasExtension(".plr"))
        {
            invalidReason = "File is not in correct format";
            return false;
        }

        return true;
    }

    private static bool TryReadPlayerInfo(string path, [NotNullWhen(true)] out TerrariaPlayerInfo? playerInfo, [NotNullWhen(false)] out Exception? failReason)
    {
        failReason = null;
        playerInfo = null;
        try
        {
            using var readStream = File.OpenRead(path);
            var reader = new TerrariaPlayerInfoReader(readStream);
            playerInfo = reader.ReadInfo();
            return true;
        }
        catch (Exception ex)
        {
            failReason = ex;
            return false;
        }
    }

    private static bool TrySavePlayerInfo(string path, TerrariaPlayerInfo playerInfo, [NotNullWhen(false)] out Exception? failReason)
    {
        failReason = null;
        try
        {
            using var fileWriteStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

            var writer = new TerrariaPlayerInfoWriter(fileWriteStream);

            writer.WritePlayerInfo(playerInfo);
            return true;
        }
        catch (Exception ex)
        {
            failReason = ex;
            return false;
        }
    }
}
