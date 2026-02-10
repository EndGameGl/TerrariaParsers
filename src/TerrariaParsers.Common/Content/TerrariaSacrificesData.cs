using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Delegates;
using TerrariaParsers.Common.Models;

namespace TerrariaParsers.Common.Content;

public static class TerrariaSacrificesData
{
    public static Dictionary<string, int> SacrificesData { get; } = [];
    public static Dictionary<TerrariaItems, int> PreprocessedSacrificesData { get; } = [];

    static TerrariaSacrificesData()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
            AllowComments = true,
            Delimiter = "\t",
        };

        using var reader = new StreamReader("Terraria.GameContent.Creative.Content.Sacrifices.tsv");
        using var csv = new CsvReader(reader, config);

        // skipping comments
        csv.Read();
        csv.Read();

        while (csv.Read())
        {
            var itemName = csv.GetField<string>(0)!;
            var itemCountAlias = csv.GetField<string>(1)!.ToLowerInvariant();

            var count = AliasToCount(itemCountAlias);

            if (count < 0)
                continue;

            SacrificesData[itemName] = count;
            PreprocessedSacrificesData[Enum.Parse<TerrariaItems>(itemName)] = count;
        }
    }

    private static int AliasToCount(string alias) =>
        alias switch
        {
            "" => 50,
            "a" => 50,
            "b" => 25,
            "c" => 5,
            "d" => 1,
            "e" => -1,
            "f" => 2,
            "g" => 3,
            "h" => 10,
            "i" => 15,
            "j" => 30,
            "k" => 99,
            "l" => 100,
            "m" => 200,
            "n" => 20,
            "o" => 400,
            _ => throw new ArgumentException($"Unknown item sacrifice category: {alias}"),
        };
}
