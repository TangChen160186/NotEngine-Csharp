namespace NotEngine.Rendering;

public abstract class UniformBuffer: IDisposable
{
    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();

    public abstract void SetSubData(IntPtr data, int size, int offset);
}