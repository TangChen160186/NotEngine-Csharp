using MessagePack;
using NotEngine.Configs;
using System.Collections.Concurrent;

namespace NotEngine.Assets;

// 定义一个委托类型，用于事件处理程序    

public class AssetManager<T> where T : class, IAsset
{
    public event AssetReloadEventHandler<T>? AssetReload;

    private readonly ConcurrentDictionary<Guid, (T asset, int refCount)> _resources = new();
    private static readonly Lazy<AssetManager<T>> _instance = new(() => new AssetManager<T>());// 懒加载
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

            OnAssetReload(new ReloadEventArgs<T>(loadedAsset,assetId));
        }
    }

    private T? LoadResource(Guid assetId)
    {
        // 假设资源存储在一个特定位置，加载资源的实际实现
        var path = Path.Combine(AssetConfig.AssetFolderPath, $"{assetId}.asset");
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            return MessagePackSerializer.Deserialize<T>(bytes);
        }
        return null;
    }

    private  void OnAssetReload(ReloadEventArgs<T> args)
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