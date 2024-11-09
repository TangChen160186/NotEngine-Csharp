using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GLVertexArray : VertexArray
{
    private int _id;
    public override bool IsDisposed { get; protected set; }

    internal GLVertexArray()
    {
        GL.CreateVertexArray(out _id);
    }

    public override void BindVertexBuffer(params VertexBuffer[] vertexBuffers)
    {
        uint index = 0;
        foreach (var vertexBuffer in vertexBuffers)
        {
            uint offset = 0;
            int stride = vertexBuffer.GetStride();

            GLVertexBuffer glVertexBuffer = (vertexBuffer as GLVertexBuffer)!;
            foreach (var layout in vertexBuffer.Layouts)
            {
                GL.VertexArrayVertexBuffer(_id, index, glVertexBuffer.Id, 0, stride);
                GL.EnableVertexArrayAttrib(_id, layout.AttributeLocation); // 使用 attributeIndex

                GL.VertexArrayAttribFormat(_id, layout.AttributeLocation, layout.Count, VertexAttribType.Float,
                    layout.IsNormalize, offset);
                GL.VertexArrayAttribBinding(_id, layout.AttributeLocation, index);

                offset += (uint)layout.Count * sizeof(float); // 偏移量按布局计算
            }

            index++;
        }
    }

    public override void BindIndexBuffer(IndexBuffer indexBuffer)
    {
        GLIndexBuffer glIndexBuffer = (indexBuffer as GLIndexBuffer)!;
        GL.VertexArrayElementBuffer(_id, glIndexBuffer.Id);
    }

    public override void Bind() => GL.BindVertexArray(_id);

    public override void Unbind() => GL.BindVertexArray(0);


    public override void Dispose()
    {
        if (!IsDisposed && _id != 0)
        {
            GL.DeleteVertexArray(in _id);
            IsDisposed = true;
            _id = 0;
        }
    }
}