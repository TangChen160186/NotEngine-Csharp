namespace NotEngine.Assets;

public enum EAssetType
{
    Shader,
    Texture2D,
    Mesh,
    Material,
}

public interface IAsset : IDisposable
{
    public Guid AssetId { get; }
    EAssetType Type { get; }
    public int RefCount { get; set; }
}