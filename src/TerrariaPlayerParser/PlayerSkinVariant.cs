namespace TerrariaParsers.Player;

public enum PlayerSkinVariant : int
{
    MaleStarter = 0,

    MaleSticker = 1,

    MaleGangster = 2,

    MaleCoat = 3,

    FemaleStarter = 4,

    FemaleSticker = 5,

    FemaleGangster = 6,

    FemaleCoat = 7,

    MaleDress = 8,

    FemaleDress = 9,

    MaleDisplayDoll = 10,

    FemaleDisplayDoll = 11,
}

public static class PlayerSkinVariantHelpers
{
    private static readonly HashSet<PlayerSkinVariant> _maleSkinVariants =
    [
        PlayerSkinVariant.MaleStarter,
        PlayerSkinVariant.MaleSticker,
        PlayerSkinVariant.MaleGangster,
        PlayerSkinVariant.MaleCoat,
        PlayerSkinVariant.MaleDress,
        PlayerSkinVariant.MaleDisplayDoll,
    ];

    public static bool IsMaleSkinVariant(PlayerSkinVariant playerSkinVariant)
    {
        return _maleSkinVariants.Contains(playerSkinVariant);
    }
}
