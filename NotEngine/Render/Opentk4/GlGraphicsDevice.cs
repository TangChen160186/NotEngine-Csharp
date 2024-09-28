namespace NotEngine.Render.Opentk4;

internal class GlGraphicsDevice: GraphicsDevice
{
    public override IndexBuffer CreateIndexBuffer(uint[] data)
    {
        return new GlIndexBuffer(data);
    }

    public override VertexBuffer CreateIndexBuffer(float[] data)
    {
        return new GlVertexBuffer(data);
    }
}