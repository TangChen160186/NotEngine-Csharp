using NotEngine.Render.Opentk4;

namespace NotEngine.Render;

internal abstract class GraphicsDevice
{   
    public GraphicsDevice CreateGraphicsDevice(RenderApi renderApi)
    {
        switch (renderApi)
        {
            case RenderApi.Opengl4:
                return new GlGraphicsDevice();
            default:
                throw new ArgumentOutOfRangeException(nameof(renderApi), renderApi, null);
        }
    }

    public abstract IndexBuffer CreateIndexBuffer(uint[] data);
    public abstract VertexBuffer CreateIndexBuffer(float[] data);
}