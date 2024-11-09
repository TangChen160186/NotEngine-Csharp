using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GLVertexBuffer : VertexBuffer
{
    private  int _id;
    internal int Id => _id;
    public override bool IsDisposed { get; protected set; }

    public override VertexBufferLayout[] Layouts { get; protected set; }
    internal GLVertexBuffer(float[] data, params VertexBufferLayout[] layouts)
    {
        GL.CreateBuffer(out _id);
        GL.NamedBufferStorage(_id, sizeof(float) * data.Length, data, BufferStorageMask.DynamicStorageBit);
        Layouts = layouts;
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