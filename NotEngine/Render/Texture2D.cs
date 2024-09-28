namespace NotEngine.Render;

internal abstract class Texture2D: IDisposable
{
    public bool IsDisposed { get; protected set; }
    protected virtual void ReleaseUnmanagedResources()
    {
        IsDisposed = true;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Texture2D()
    {
        ReleaseUnmanagedResources();
    }
}