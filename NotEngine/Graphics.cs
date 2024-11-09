using NotEngine.Assets;
using NotEngine.Rendering;
using NotEngine.Rendering.Opengl;

namespace NotEngine;

public static class Graphics
{
    public static GraphicsDevice Device { get; internal set; } = null!;

    public static void Initialize()
    {
        Device = new GLDevice();
    }

    public static unsafe void DrawMesh(Mesh mesh, Material material)
    {
        mesh.Bind();
        material.Apply();
        Graphics.Device.SetRasterizerState(material.RasterizerState);
        Graphics.Device.DrawIndex(EPrimitiveType.TriangleStrip,mesh.IndexCount,true,null);
    }

}