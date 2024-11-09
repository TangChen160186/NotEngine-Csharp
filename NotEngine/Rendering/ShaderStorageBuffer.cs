namespace NotEngine.Rendering;

public abstract class ShaderStorageBuffer: IDisposable
{
    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();
    public abstract void SetData(IntPtr data, int size);
}