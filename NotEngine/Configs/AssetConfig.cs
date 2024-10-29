namespace NotEngine.Configs;

public static class AssetConfig
{
    public static string AssetFolderPath { get; private set; } = "";

    public static void Init(string assetFolderPath)
    {
        if (string.IsNullOrEmpty(AssetFolderPath))
        {
            AssetFolderPath = assetFolderPath;
        }
        else
        {
            throw new InvalidOperationException("Asset folder path has already been initialized.");
        }
    }
}