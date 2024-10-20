using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public sealed class UniformBuffer : IDisposable
{
    private readonly int _id;
    public bool IsDisposed { get; private set; }

    public unsafe UniformBuffer(int size, uint bindingPoint = 0)
    {
        GL.CreateBuffer(out _id);
        GL.NamedBufferData(_id, size, null, VertexBufferObjectUsage.DynamicDraw);
        GL.BindBufferRange(BufferTarget.UniformBuffer, bindingPoint, _id, 0, size);
    }

    public void SetSubData(IntPtr data, int size, int offset)
        => GL.NamedBufferSubData(_id, offset, size, data);


    //public static void BindBlockToShader(ShaderProgram shader, uint uniformBlockIndex, uint bindingPoint) =>
    //    GL.UniformBlockBinding(shader._id, uniformBlockIndex, bindingPoint);


    //public static void BindBlockToShader(ShaderProgram shader, string name, uint bindingPoint = 0) =>
    //    GL.UniformBlockBinding(shader._id, GetUniformBlockIndex(shader, name), bindingPoint);


    //private static uint GetUniformBlockIndex(ShaderProgram shader, string name) => GL.GetUniformBlockIndex(shader._id, name);

    public void Dispose()
    {
        if (!IsDisposed)
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }


    ~UniformBuffer() => ReleaseUnmanagedResources();
    private void ReleaseUnmanagedResources() =>
        GL.DeleteBuffer(in _id);
}