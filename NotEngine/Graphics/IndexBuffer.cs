using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public sealed class IndexBuffer : IDisposable
{
    private readonly int _id;
    internal int Id => _id;
    public bool IsDisposed { get; private set; }
    public IndexBuffer(uint[] data)
    {
        GL.CreateBuffer(out _id);
        GL.NamedBufferStorage(_id, sizeof(uint) * data.Length, data, BufferStorageMask.DynamicStorageBit);
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }

    ~IndexBuffer() => ReleaseUnmanagedResources();
    private void ReleaseUnmanagedResources() => GL.DeleteBuffer(in _id);
}