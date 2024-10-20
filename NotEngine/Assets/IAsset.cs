namespace NotEngine.Assets;

public abstract class AssetBase : IDisposable
{   
    public abstract bool IsDisposed { get; protected set; }
    public Guid AssetId { get;internal set; } = new Guid();
    public abstract EAssetType Type { get; internal set; }
    public int RefCount { get; internal set; } = 0;
    public List<Guid> RefIds { get; internal set; } = [];
    public List<Guid> Dependencies { get; internal set; } = [];
    public string? LoadPath { get; internal set; } = null;
    public string AssetPath { get; set; } = string.Empty;
    public abstract string Name { get; internal set; }
    public abstract void Dispose();
}
