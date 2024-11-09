using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GLShaderStorageBuffer : ShaderStorageBuffer
{
    private int _id;
    public override bool IsDisposed { get; protected set; }
    internal GLShaderStorageBuffer(uint bindingIndex)
    {
        GL.CreateBuffer(out _id);
        GL.BindBufferBase(BufferTarget.ShaderStorageBuffer, bindingIndex, _id);
    }

    public override void SetData(nint data, int size)
        => GL.NamedBufferData(_id, size, data, VertexBufferObjectUsage.DynamicDraw);


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