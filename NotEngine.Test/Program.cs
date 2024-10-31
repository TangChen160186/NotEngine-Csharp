using System.Numerics;
using MessagePack;
using NotEngine.Assets;
using NotEngine.Configs;
using NotEngine.ECS;
using NotEngine.ECS.Components;
using NotEngine.ECS.Systems;
using NotEngine.Graphics;
using OpenTK.Core.Utility;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace NotEngine.Test;

partial class Sample
{
    public static unsafe void Main(string[] args)
    {
        
        EventQueue.EventRaised += EventRaised;
        ToolkitOptions options = new ToolkitOptions
        {
            ApplicationName = "OpenTK tutorial",
            Logger = new ConsoleLogger()
        };

        Toolkit.Init(options);

        OpenGLGraphicsApiHints contextSettings = new OpenGLGraphicsApiHints()
        {
            // Here different options of the opengl context can be set.
            Version = new Version(4, 6),
            Profile = OpenGLProfile.Core,
            DebugFlag = true,
            DepthBits = ContextDepthBits.Depth32,
            StencilBits = ContextStencilBits.Stencil8,
        };

        WindowHandle window = Toolkit.Window.Create(contextSettings);
        
        OpenGLContextHandle glContext = Toolkit.OpenGL.CreateFromWindow(window);
        // The the current opengl context and load the bindings.
        Toolkit.OpenGL.SetCurrentContext(glContext);
        GLLoader.LoadBindings(Toolkit.OpenGL.GetBindingsContext(glContext));

        // Show window
        Toolkit.Window.SetTitle(window, "OpenTK window");
        Toolkit.Window.SetSize(window, new Vector2i(800, 600));
        Toolkit.Window.SetMode(window, WindowMode.Normal);

        ShaderProgram shader = new ShaderProgram(File.ReadAllText("Shaders/Test.glsl"));

        Mesh quad = new Mesh([
            new Vertex() { Position = new Vector3(1,1,0), TexCoords = new Vector2(1,1) },
            new Vertex() { Position = new Vector3(1,-1,0), TexCoords = new Vector2(1,0) },
            new Vertex() { Position = new Vector3(-1,-1,0), TexCoords = new Vector2(0,0) },
            new Vertex() { Position = new Vector3(-1,1,0), TexCoords = new Vector2(0,1) },
        ], [0, 1, 3, 
            1, 2, 3 ],"");

        ShaderProgram shader1 = new ShaderProgram(File.ReadAllText("Shaders/Quad.glsl"));
   
        var guid = TextureImporter.Import(@"C:\Users\16018\Desktop\TextureTest\source.png", 
            @$"C:\Users\16018\Desktop\TextureTest",isCompressed:true,isSrgb:false);
        AssetConfig.Init(@"C:\Users\16018\Desktop\TextureTest\");


        AssetRef<Texture> texture =new AssetRef<Texture>(AssetManager<Texture>.Instance.GetResource(guid));

        var bytes =File.ReadAllBytes(@"C:\Users\16018\Desktop\TextureTest\source.asset");
        //Texture texture = MessagePackSerializer.Deserialize<Texture>(bytes);


        MeshImporter.Import(@"C:\Users\16018\Desktop\Cube.fbx", @"C:\Users\16018\Desktop\TextureTest\MeshTest");
        List<Mesh> meshes = new List<Mesh>();
        var dirInfo = new DirectoryInfo(@"C:\Users\16018\Desktop\TextureTest\MeshTest");
        foreach (var fileInfo in dirInfo.EnumerateFiles())
        {
            bytes = File.ReadAllBytes(fileInfo.FullName);
            Mesh mesh = MessagePackSerializer.Deserialize<Mesh>(bytes);
            meshes.Add(mesh);
        }

        frameBuffer = new FrameBuffer(800, 600, 1);

        var material = new Material(new Shader(File.ReadAllText("Shaders/Test.glsl")));
        AssetRef<Texture> teRef = null;
        while (true)
        {
            // This will process events for all windows and
            // post those events to the event queue.
            Toolkit.Window.ProcessEvents(false);
            if (Toolkit.Window.IsWindowDestroyed(window))
            {
                break;
            }

            frameBuffer?.Bind();
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.StencilTest);
            GL.Enable(EnableCap.Blend);
            GL.ClearColor(new Color4<Rgba>(64 / 255f, 0, 127 / 255f, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);

            foreach (var mesh in meshes)
            {
                mesh.Bind();
                shader.Bind();
                shader.SetTextureHandle("u_DiffuseMap", texture.Asset.TextureHandleId);
                shader.SetUniform("model", Matrix4x4.Identity * Matrix4x4.CreateRotationX((float)(Math.PI * 30 / 180f)) * 
                                           Matrix4x4.CreateRotationY((float)(Math.PI * 30 / 180f)));
                shader.SetUniform("view", Matrix4x4.CreateLookAt(new Vector3(0, 0, 500), Vector3.Zero, Vector3.UnitY));
                Toolkit.Window.GetFramebufferSize(window, out var size);
                shader.SetUniform("projection", Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI * 45 / 180f),
                    size.X * 1.0f / size.Y, 0.1f, 1000f));  
                GL.DrawElements(PrimitiveType.Triangles,mesh.IndexCount,DrawElementsType.UnsignedInt, 0);

            }

            Toolkit.OpenGL.SwapBuffers(glContext);
            frameBuffer?.Unbind();
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.StencilTest);
            GL.Disable(EnableCap.Blend);

            quad.Bind();
            shader1.Bind();
            shader1.SetTextureHandle("yourTexture", frameBuffer.ColorAttachments![0].TextureHandleId);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
    }

    private static FrameBuffer frameBuffer;
    static void EventRaised(PalHandle? handle, PlatformEventType type, EventArgs args)
    {
        if (args is CloseEventArgs closeArgs)
        {
            // Destroy the window that the user wanted to close.
            Toolkit.Window.Destroy(closeArgs.Window);
        }
        
        if (args is WindowFramebufferResizeEventArgs resizeEventArgs)
        {
            var m = resizeEventArgs.NewFramebufferSize;
            GL.Viewport(0, 0, resizeEventArgs.NewFramebufferSize.X, resizeEventArgs.NewFramebufferSize.Y);
            if (frameBuffer != null)
                frameBuffer.Dispose();
            frameBuffer = new FrameBuffer(resizeEventArgs.NewFramebufferSize.X, resizeEventArgs.NewFramebufferSize.Y, 1);
            GC.Collect();
        }
    }
}