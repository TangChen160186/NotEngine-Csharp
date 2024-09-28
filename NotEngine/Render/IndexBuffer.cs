namespace NotEngine.Render;

internal abstract class IndexBuffer: IDisposable
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

    ~IndexBuffer()
    {
        ReleaseUnmanagedResources();
    }
}