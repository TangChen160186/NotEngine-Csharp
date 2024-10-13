namespace NotEngine.Assets;

public class Texture2D: IAsset
{
    public Guid AssetId { get; }
    public EAssetType Type => EAssetType.Texture2D;
    public int RefCount { get; set; }


    public void Dispose()
    {
        // TODO 在此释放托管资源
    }

}