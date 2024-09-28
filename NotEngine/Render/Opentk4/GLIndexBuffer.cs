using OpenTK.Graphics.OpenGL;

namespace NotEngine.Render.Opentk4;

internal sealed class GlIndexBuffer: IndexBuffer
{
    private readonly int _bufferHandle;
    public GlIndexBuffer(uint[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        GL.CreateBuffer(out _bufferHandle);
        GL.NamedBufferData(_bufferHandle,sizeof(uint)* data.Length,data, VertexBufferObjectUsage.StaticDraw);
    }

    protected override void ReleaseUnmanagedResources()
    {
        GL.DeleteBuffer(in _bufferHandle);
        base.ReleaseUnmanagedResources();
    }
}