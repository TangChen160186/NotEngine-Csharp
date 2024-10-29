using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public sealed class ShaderStorageBuffer : IDisposable
{
    private readonly int _id;
    public bool IsDisposed { get; private set; }
    public ShaderStorageBuffer(uint bindingIndex)
    {
        GL.CreateBuffer(out _id);
        GL.BindBufferBase(BufferTarget.ShaderStorageBuffer, bindingIndex, _id);
    }
        
    public void SetData(IntPtr data,int size)
        =>GL.NamedBufferData(_id, size, data, VertexBufferObjectUsage.DynamicDraw);


    public void Dispose()
    {
        if (!IsDisposed)
        {
            GL.DeleteBuffer(in _id);
            IsDisposed = true;
        }
    }
}