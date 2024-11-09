using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GLUniformBuffer : UniformBuffer
{
    private  int _id;
    public override bool IsDisposed { get; protected set; }

    internal unsafe GLUniformBuffer(int size, uint bindingPoint = 0)
    {
        GL.CreateBuffer(out _id);
        GL.NamedBufferData(_id, size, null, VertexBufferObjectUsage.DynamicDraw);
        GL.BindBufferRange(BufferTarget.UniformBuffer, bindingPoint, _id, 0, size);
    }

    public override void SetSubData(nint data, int size, int offset)
        => GL.NamedBufferSubData(_id, offset, size, data);
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