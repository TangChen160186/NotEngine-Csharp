namespace NotEngine.Rendering;

public abstract class IndexBuffer : IDisposable
{
    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();
}