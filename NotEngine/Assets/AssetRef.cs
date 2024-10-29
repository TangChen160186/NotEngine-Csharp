using MessagePack;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true)]
public struct AssetRef<T> where T : class, IAsset
{
    public Guid AssetId => Asset?.AssetId ?? Guid.Empty;
    [IgnoreMember]
    public T? Asset { get; private set; }
    public bool HasValue => Asset != null;

    [SerializationConstructor]
    public AssetRef(Guid assetId)
    {
        if (assetId != Guid.Empty)
        {
            Asset = AssetManager<T>.Instance.GetResource(assetId);
            AssetManager<T>.Instance.IncrementReferenceCount(AssetId);
            AssetManager<T>.Instance.AssetReload += OnAssetReload;
        }
    }

    public AssetRef(T? asset = null)
    {
        Asset = asset;
        if (asset != null)
        {
            AssetManager<T>.Instance.IncrementReferenceCount(AssetId);
            AssetManager<T>.Instance.AssetReload += OnAssetReload;
        }
    }


    private void OnAssetReload(ReloadEventArgs<T> args)
    {
        if (args.AssetId == AssetId)
        {
            Asset = args.Asset;
        }
    }


    public void Dispose()
    {
        Release();
    }

    private void Release()
    {
        if (Asset != null)
        {
            AssetManager<T>.Instance.DecrementReferenceCount(AssetId);
            AssetManager<T>.Instance.AssetReload -= OnAssetReload;
            Asset = null;
        }
    }

    public static implicit operator AssetRef<T>(T? d) => new AssetRef<T>(d);

}