namespace NotEngine.Render;

internal class VertexBuffer: IDisposable
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

    ~VertexBuffer()
    {
        ReleaseUnmanagedResources();
    }
}