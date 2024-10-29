using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public sealed class VertexArray : IDisposable
{
    private readonly int _id;
    public bool IsDisposed { get; private set; }

    public VertexArray()
    {
        GL.CreateVertexArray(out _id);
    }

    public void BindVertexBuffer(params VertexBuffer[] vertexBuffers)
    {
        uint index = 0;
        foreach (var vertexBuffer in vertexBuffers)
        {
      
            uint offset = 0;
            int stride = vertexBuffer.GetStride();
            foreach (var layout in vertexBuffer.Layouts)    
            {
                GL.VertexArrayVertexBuffer(_id, index, vertexBuffer.Id, 0, stride);
                GL.EnableVertexArrayAttrib(_id, layout.AttributeLocation); // 使用 attributeIndex

                GL.VertexArrayAttribFormat(_id, layout.AttributeLocation, layout.Count, VertexAttribType.Float,
                    layout.IsNormalize, offset);
                GL.VertexArrayAttribBinding(_id, layout.AttributeLocation, index);

                offset += (uint)layout.Count * sizeof(float); // 偏移量按布局计算
            }

            index++;
        }
    }

    public void BindIndexBuffer(IndexBuffer indexBuffer)
    {
        GL.VertexArrayElementBuffer(_id, indexBuffer.Id);
    }

    public void Bind() => GL.BindVertexArray(_id);

    public void Unbind() => GL.BindVertexArray(0);


    public void Dispose()
    {
        if (!IsDisposed)
        {
            GL.DeleteVertexArray(in _id);
            IsDisposed = true;
        }
    }
}