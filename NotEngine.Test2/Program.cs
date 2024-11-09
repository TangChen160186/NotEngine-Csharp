using MessagePack;
using NotEngine.Assets;
using NotEngine.ECS;
using NotEngine.ECS.Components;
using NotEngine.ECS.Systems;
using NotEngine.Rendering;
using OpenTK.Compute.OpenCL;
using OpenTK.Core.Utility;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using OpenTK.Windowing.Common;

namespace NotEngine.Test2;
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

        Graphics.Initialize();
        TextureImporter.Import(@"C:\Users\16018\Desktop\ibl_hdr_radiance.png", @"C:\Users\16018\Desktop\TextureTest",
            false, CompressFormat.BC1, false, true);

        var texture = MessagePackSerializer.Deserialize<Texture2D>(
            File.ReadAllBytes(@"C:\Users\16018\Desktop\TextureTest\BC1.asset"));

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
        actor.AddComponent<MeshRenderComponent>().Material = new Material(new Shader(File.ReadAllText("Shaders/Test.glsl")));
        actor.GetComponent<MeshRenderComponent>().Material.Asset.RasterizerState.Blendable = false;
        actor.GetComponent<MeshRenderComponent>().Material.Asset.SetUniform("u_DiffuseMap", new AssetRef<Texture2D>(texture));
        TransformSystem system = new TransformSystem(_scene);
        RenderSystem renderSystem = new RenderSystem(_scene);


        system.Run();


        RenderTexture(texture);

        while (true)
        {
            Toolkit.Window.ProcessEvents(false);
            if (Toolkit.Window.IsWindowDestroyed(window))
            {
                break;
            }
            Graphics.Device.ClearColorTarget(0, 64 / 255f, 0, 127 / 255f, 255);
            Graphics.Device.ClearDepthStencil(1,0);
            //actor.GetComponent<TransformComponent>()!.WorldRotation *= Quaternion.CreateFromYawPitchRoll(0, MathF.PI * 0.01f / 180, 0);
            //system.Run();
            //renderSystem.Run();
            Toolkit.OpenGL.SwapBuffers(glContext);
        }
    }
    public static void RenderTexture(Texture2D texture)
    {
        GraphicTexture2D depthTexture2D = Graphics.Device.CreateGraphicTexture2D(texture.Width, texture.Height,
            TextureInternalFormat.Depth32fStencil8, false);

        GraphicTexture2D colorTexture2D = Graphics.Device.CreateGraphicTexture2D(texture.Width, texture.Height,
            TextureInternalFormat.RGBA8_UNORM, false);

        FrameBuffer frameBuffer =
            Graphics.Device.CreateFrameBuffer(texture.Width, texture.Height, depthTexture2D, [colorTexture2D]);
        ShaderProgram shaderProgram = Graphics.Device.CreateShaderProgram(File.ReadAllText("Shaders/Quad.glsl"));
        frameBuffer.Bind();
        Graphics.Device.SetViewport(0, 0, 0, texture.Width, texture.Height);
        List<Vertex> vertices =
        [
            new() { Position = new Vector3(1f, 1f, 0.0f),TexCoords = new Vector2( 1.0f, 1.0f)},
            new() { Position = new Vector3(1f, -1f, 0.0f),TexCoords = new Vector2(1.0f, 0.0f) },
            new() { Position = new Vector3(-1f, -1f, 0.0f) ,TexCoords = new Vector2(0.0f, 0.0f)},
            new() { Position = new Vector3(-1f, 1f, 0.0f),TexCoords = new Vector2(0.0f, 1.0f ) }
        ];

        uint[] indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };
        Mesh mesh = new Mesh(vertices, indices, "");
        Shader shader = new Shader(File.ReadAllText("Shaders/Quad.glsl"));
        Material material = new Material(shader);
        material.RasterizerState.DepthTest = false;
        material.RasterizerState.DepthWriting = false;
        material.RasterizerState.Blendable = false;
        material.SetUniform("yourTexture", new AssetRef<Texture2D>(texture));
        for (int i = 0; i < texture.MipLevels; i++)
        {
            material.SetUniform("level", i);
            Graphics.DrawMesh(mesh, material);
            var pixelData = Graphics.Device.ReadPixel(0, 0, 0, texture.Width, texture.Height);

            // Step 4: Convert pixel data to an Image<Rgba32> object
            using Image<Rgba32> image = Image.LoadPixelData<Rgba32>(pixelData, texture.Width, texture.Height);
            // ImageSharp loads the image data with the origin at the top-left corner, 
            // so we may need to flip the image vertically to match the framebuffer layout
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            // Step 5: Save the image to a file 
            image.Save($@"C:\Users\16018\Desktop\fa\res{i}.png"); // Save as PNG or specify format in file path like "filePath.png"
        }
        frameBuffer.Unbind();
        depthTexture2D.Dispose();
        colorTexture2D.Dispose();
        frameBuffer.Dispose();
        shaderProgram.Dispose();
        shader.Dispose();
        material.Dispose();
        mesh.Dispose();
    }
    static void EventRaised(PalHandle? handle, PlatformEventType type, EventArgs args)
    {
        if (args is CloseEventArgs closeArgs)
        {
            // Destroy the window that the user wanted to close.
            Toolkit.Window.Destroy(closeArgs.Window);
        }

        if (args is WindowFramebufferResizeEventArgs resizeEventArgs)
        {
            Graphics.Device.SetViewport(0, 0, 0, resizeEventArgs.NewFramebufferSize.X, resizeEventArgs.NewFramebufferSize.Y);
            _scene.UpdateSceneSize(resizeEventArgs.NewFramebufferSize.X, resizeEventArgs.NewFramebufferSize.Y);
        }
    }
}