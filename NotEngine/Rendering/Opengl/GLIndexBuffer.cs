using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GLIndexBuffer : IndexBuffer
{
    private int _id;
    internal int Id => _id;
    public override bool IsDisposed { get; protected set; }
    internal GLIndexBuffer(uint[] data)
    {
        GL.CreateBuffer(out _id);
        GL.NamedBufferStorage(_id, sizeof(uint) * data.Length, data, BufferStorageMask.DynamicStorageBit);
    }


    public override void Dispose()
    {
        if (!IsDisposed && _id != 0)
        {
            GL.DeleteBuffer(in _id);
            IsDisposed = true;
            _id = 0;
        }
    }
}