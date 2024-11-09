namespace NotEngine.Rendering;

public abstract class FrameBuffer: IDisposable
{
    public abstract bool IsDisposed { get; protected set; }

    public abstract void Dispose();

    public abstract void Bind();

    public abstract void Unbind(int frameBufferId = 0);

    public abstract GraphicTexture2D DepthAttachment { get; }
    public abstract GraphicTexture2D[]? ColorAttachments { get; }
}