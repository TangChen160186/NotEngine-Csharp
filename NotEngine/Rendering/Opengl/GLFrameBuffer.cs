using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GLFrameBuffer : FrameBuffer
{
    private int _id;
    public int Id => _id;
    public override GraphicTexture2D DepthAttachment { get; }
    public override GraphicTexture2D[]? ColorAttachments { get; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public override bool IsDisposed { get; protected set; }

    public GLFrameBuffer(int width, int height, GraphicTexture2D depthAttachment, GraphicTexture2D[]? colorAttachments)
    {
        Width = width;
        Height = height;
        GL.CreateFramebuffer(out _id);
        DepthAttachment = depthAttachment;
        ColorAttachments = colorAttachments;

        if (colorAttachments != null)
        {
            for (uint i = 0; i < colorAttachments.Length; i++)
            {
                var glTexture2D = (colorAttachments[i] as GlGraphicTexture2D)!;
                GL.NamedFramebufferTexture(_id, FramebufferAttachment.ColorAttachment0 + i, glTexture2D.Id, 0);
                GL.NamedFramebufferDrawBuffer(_id, ColorBuffer.ColorAttachment0 + i);
            }
        }
        else
        {
            GL.NamedFramebufferDrawBuffer(_id, ColorBuffer.None);
            GL.NamedFramebufferReadBuffer(_id, ColorBuffer.None);
        }
        var glDepthTexture2D = (depthAttachment as GlGraphicTexture2D)!;
        GL.NamedFramebufferTexture(_id, FramebufferAttachment.DepthStencilAttachment, glDepthTexture2D.Id, 0);

        var status = GL.CheckNamedFramebufferStatus(_id, FramebufferTarget.Framebuffer);

        if (status != FramebufferStatus.FramebufferComplete)
        {
            throw new Exception($"The FrameBuffer is not complete: {status}.");
        }
    }


    public override void Bind() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
    public override void Unbind(int frameBufferId = 0) => GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

    public override void Dispose()
    {
        if (!IsDisposed && _id != 0)
        {
            GL.DeleteFramebuffer(in _id);
            IsDisposed = true;
            _id = 0;
        }
    }
}