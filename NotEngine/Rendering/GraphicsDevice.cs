namespace NotEngine.Rendering;

public abstract class GraphicsDevice: IDisposable
{

    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();

    public abstract VertexBuffer CreateVertexBuffer(float[] data, params VertexBufferLayout[] layouts);

    public abstract IndexBuffer CreateIndexBuffer(uint[] data);

    public abstract VertexArray CreateVertexArray();

    public abstract UniformBuffer CreateUniformBuffer(int size, uint bindingPoint = 0);

    public abstract ShaderStorageBuffer CreateShaderStorageBuffer(uint bindingIndex);

    public abstract ShaderProgram CreateShaderProgram(string shaderSource);


    public abstract GraphicTexture2D CreateGraphicTexture2D(int width,
        int height,
        TextureInternalFormat format,
        bool genMipmap);

    public abstract FrameBuffer CreateFrameBuffer(int width, int height, GraphicTexture2D depthAttachment,
        GraphicTexture2D[]? colorAttachments);
    public abstract void ClearColorTarget(int index, float r, float g, float b, float a);
    public abstract void ClearDepthStencil(float depth, uint stencil);
    public abstract void ClearColorTarget(FrameBuffer frameBuffer, int index, float r, float g, float b, float a);  
    public abstract void ClearDepthStencil(FrameBuffer frameBuffer, float depth, uint stencil);
    public abstract void SetViewport(uint index, float x, float y, float width, float height);

    public abstract void SetRasterizerState(RasterizerState state);


    public abstract void DrawArrays(EPrimitiveType primitiveType, int first, int count);
    public abstract unsafe void DrawIndex(EPrimitiveType primitiveType, int indexCount, bool index32Bit, void* value);

    public abstract byte[] ReadPixel(uint index, int x, int y, int width, int height);
}