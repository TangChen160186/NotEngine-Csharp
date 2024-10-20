using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public sealed class FrameBuffer : IDisposable
{
    private readonly int _id;
    public GraphicTexture2D DepthAttachment { get; }
    public GraphicTexture2D[]? ColorAttachments { get; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public bool IsDisposed { get; private set; }

    public FrameBuffer(int width, int height, uint colorAttachmentCount = 1)
    {
        Width = width;
        Height = height;
        GL.CreateFramebuffer(out _id);
        DepthAttachment = new GraphicTexture2D(width, height, false, false, 1, SizedInternalFormat.Depth32fStencil8);
        if (colorAttachmentCount > 0)
        {
            ColorAttachments = new GraphicTexture2D[colorAttachmentCount];
            for (int i = 0; i < colorAttachmentCount; i++)
            {
                ColorAttachments[i] = new GraphicTexture2D(width, height, false, false, 1, SizedInternalFormat.Rgba8);
            }
        }


        for (uint i = 0; i < colorAttachmentCount; i++)
        {
            GL.NamedFramebufferTexture(_id, FramebufferAttachment.ColorAttachment0 + i, ColorAttachments![i].Id, 0);
        }
        GL.NamedFramebufferTexture(_id, FramebufferAttachment.DepthStencilAttachment, DepthAttachment.Id, 0);


        if (colorAttachmentCount > 0)
        {
            for (uint i = 0; i < colorAttachmentCount; i++)
            {
                GL.NamedFramebufferDrawBuffer(_id,ColorBuffer.ColorAttachment0 + i);
            }   
        }
        else
        {
            GL.NamedFramebufferDrawBuffer(_id, ColorBuffer.None);
            GL.NamedFramebufferReadBuffer(_id, ColorBuffer.None);
        }
        var status = GL.CheckNamedFramebufferStatus(_id, FramebufferTarget.Framebuffer);
        
        if (status != FramebufferStatus.FramebufferComplete)
        {
            throw new Exception($"The FrameBuffer is not complete: {status}.");
        }
    }


    public void Bind() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
    public void Unbind(int frameBufferId = 0) => GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);


    public void Dispose()
    {
        if (!IsDisposed)
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }

    ~FrameBuffer()
    {
        ReleaseUnmanagedResources();
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteFramebuffer(in _id);
        DepthAttachment.Dispose();
        if (ColorAttachments != null)
        {
            foreach (var attachment in ColorAttachments)
            {
                attachment.Dispose();
            }
        }
    }
}