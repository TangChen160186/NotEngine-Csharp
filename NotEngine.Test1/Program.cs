using MessagePack;
using NotEngine.Assets;
using NotEngine.ECS;
using NotEngine.ECS.Components;
using NotEngine.ECS.Systems;
using NotEngine.Rendering;
using OpenTK.Core.Utility;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace NotEngine.Test1;
internal class Sample
{
    static Scene _scene = new Scene();

    
    public static void Main(string[] args)
    {
        #region Init


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
        EventQueue.EventRaised += EventRaised;
        #endregion


        TextureImporter.Import(@"C:\Users\16018\Desktop\ibl_hdr_radiance.png", @"C:\Users\16018\Desktop\TextureTest",
            true, CompressFormat.BC3, false, false);

        var texture =MessagePackSerializer.Deserialize<Texture2D>(
            File.ReadAllBytes(@"C:\Users\16018\Desktop\TextureTest\BC7.asset"));
        
        MeshImporter.Import(@"C:\Users\16018\Desktop\Cube.fbx", @"C:\Users\16018\Desktop\MeshTest");
        List<Mesh> meshes = new List<Mesh>();
        var dirInfo = new DirectoryInfo(@"C:\Users\16018\Desktop\MeshTest");
        foreach (var fileInfo in dirInfo.EnumerateFiles())
        {
            var bytes = File.ReadAllBytes(fileInfo.FullName);
            Mesh mesh = MessagePackSerializer.Deserialize<Mesh>(bytes);
            meshes.Add(mesh);
        }

        var camera = _scene.CreateActor();
        camera.AddComponent<CameraComponent>();
        camera.GetComponent<TransformComponent>()!.WorldPosition = new Vector3(0, 0, -10);
   


        var actor = _scene.CreateActor();
        actor.AddComponent<MeshFilterComponent>().Mesh = meshes[0];
        actor.AddComponent<MeshRenderComponent>().Material =new Material(new Shader(File.ReadAllText("Shaders/Test.glsl")));
        actor.GetComponent<MeshRenderComponent>().Material.Asset.Blendable = false;
        actor.GetComponent<MeshRenderComponent>().Material.Asset.SetUniform("u_DiffuseMap", new AssetRef<Texture2D>(texture));
        TransformSystem system = new TransformSystem(_scene);
        RenderSystem renderSystem = new RenderSystem(_scene);


        system.Run();
        while (true)
        {
            Toolkit.Window.ProcessEvents(false);
            if (Toolkit.Window.IsWindowDestroyed(window))
            {
                break;
            }
            GL.ClearColor(new Color4<Rgba>(64 / 255f, 0, 127 / 255f, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);
            actor.GetComponent<TransformComponent>()!.WorldRotation *= Quaternion.CreateFromYawPitchRoll(0, MathF.PI * 0.01f / 180, 0);
            system.Run();
            renderSystem.Run();
            Toolkit.OpenGL.SwapBuffers(glContext);
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
           GL.Viewport(0,0,resizeEventArgs.NewFramebufferSize.X,resizeEventArgs.NewFramebufferSize.Y);
           _scene.UpdateSceneSize(resizeEventArgs.NewFramebufferSize.X, resizeEventArgs.NewFramebufferSize.Y);
        }
    }
}