namespace NotEngine.Rendering;

public abstract class VertexArray: IDisposable
{
    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();

    public abstract void BindVertexBuffer(params VertexBuffer[] vertexBuffers);
    public abstract void BindIndexBuffer(IndexBuffer indexBuffer);

    public abstract void Bind();
    public abstract void Unbind();
}