using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public struct VertexBufferLayout
{
    public uint AttributeLocation;
    public bool IsNormalize;
    public int Count;
}
public sealed class VertexBuffer : IDisposable
{
    private readonly int _id;
    public bool IsDisposed { get; private set; }
    internal int Id => _id;
    public VertexBufferLayout[] Layouts { get; private set; }
    public VertexBuffer(float[] data, params VertexBufferLayout[] layouts)
    {
        GL.CreateBuffer(out _id);
        GL.NamedBufferStorage(_id, sizeof(float) * data.Length, data, BufferStorageMask.DynamicStorageBit);
        Layouts = layouts;
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

    public int GetStride()
    {
        return (int)(Layouts.Sum(e => e.Count) * sizeof(float));
    }


    ~VertexBuffer()=> ReleaseUnmanagedResources();
    private void ReleaseUnmanagedResources() => GL.DeleteBuffer(in _id);
}