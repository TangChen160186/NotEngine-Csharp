using MessagePack;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true)]
public class MetaData(Guid id, string assetPath, string reloadPath)
{
    public Guid Id { get; set; } = id;
    public string AssetPath { get; set; } = assetPath;
    public string ReloadPath { get; set; } = reloadPath;
}