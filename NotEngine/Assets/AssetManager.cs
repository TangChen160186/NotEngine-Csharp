using MessagePack;
using NotEngine.Configs;
using System.Collections.Concurrent;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true)]
public partial class AssetMap
{
    public static string AssetFolderPath { get; set; } = "";
    private static readonly Lazy<AssetMap> _instance = new(() => new AssetMap()); // 懒加载
    public ConcurrentDictionary<Guid, MetaData> MetaDatas { get; private set; } = [];
    [IgnoreMember] public static AssetMap Instance => _instance.Value;

    public MetaData? GetMetaData(Guid id)
    {
        return MetaDatas.GetValueOrDefault(id);
    }

    public string? GetAssetPath(Guid id)
    {
        var metaData = GetMetaData(id);
        if(metaData!=null)
            return Path.Combine(AssetFolderPath,metaData.AssetPath);
        return null;
    }
}

[MessagePackObject(keyAsPropertyName: true)]
public class MetaData
{
    public MetaData(Guid id, string assetPath, string reloadPath)
    {
        Id = id;
        AssetPath = assetPath;
        ReloadPath = reloadPath;
    }

    public Guid Id { get; set; }
    public string AssetPath { get; set; }
    public string ReloadPath { get; set; }
}

public class AssetManager<T> where T : class, IAsset
{
    public event AssetReloadEventHandler<T>? AssetReload;

    private readonly ConcurrentDictionary<Guid, (T asset, int refCount)> _resources = new();
    private static readonly Lazy<AssetManager<T>> _instance = new(() => new AssetManager<T>()); // 懒加载
    public static AssetManager<T> Instance => _instance.Value;

    public T? GetResource(Guid assetId)
    {
        if (_resources.TryGetValue(assetId, out var resource))
        {
            return resource.asset;
        }

        var loadedAsset = LoadResource(assetId);
        if (loadedAsset != null)
        {
            _resources[assetId] = (loadedAsset, 0); // 默认开始计数为0
        }

        return loadedAsset;
    }

    public void IncrementReferenceCount(Guid assetId)
    {
        if (_resources.TryGetValue(assetId, out var resource))
        {
            _resources[assetId] = (resource.asset, resource.refCount + 1);
        }
    }

    public void DecrementReferenceCount(Guid assetId)
    {
        if (_resources.TryGetValue(assetId, out var resource))
        {
            var newRefCount = resource.refCount - 1;
            if (newRefCount <= 0)
            {
                resource.asset.Dispose();
                _resources.TryRemove(assetId, out _);
            }
            else
            {
                _resources[assetId] = (resource.asset, newRefCount);
            }
        }
    }

    public void UnloadUnusedResources()
    {
        foreach (var assetId in _resources.Keys)
        {
            if (_resources.TryGetValue(assetId, out var resource) && resource.refCount == 0)
            {
                resource.asset.Dispose();
                _resources.TryRemove(assetId, out _);
            }
        }
    }

    public void ReloadResource(Guid assetId)
    {
        if (_resources.TryGetValue(assetId, out var resource))
        {
            var refCount = resource.refCount;
            resource.asset.Dispose();
            _resources.Remove(assetId, out _);
            var loadedAsset = LoadResource(assetId);
            if (loadedAsset != null)
            {
                _resources[assetId] = (loadedAsset, refCount);
            }

            OnAssetReload(new ReloadEventArgs<T>(loadedAsset, assetId));
        }
    }

    private T? LoadResource(Guid assetId)
    {
        var metaData = AssetMap.Instance.GetMetaData(assetId);
        if (metaData == null) return null;

        var path = Path.Combine(AssetConfig.AssetFolderPath, metaData.AssetPath);
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        return null;
    }

    private void OnAssetReload(ReloadEventArgs<T> args)
    {
        AssetReload?.Invoke(args);
    }
}

public class ReloadEventArgs<T>(T? asset, Guid assetId)
    where T : class, IAsset
{
    public Guid AssetId { get; } = assetId;
    public T? Asset { get; } = asset;
}

public delegate void AssetReloadEventHandler<T>(ReloadEventArgs<T> args) where T : class, IAsset;