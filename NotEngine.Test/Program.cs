using NotEngine.Graphics;
using OpenTK.Core.Utility;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using StbImageSharp;

namespace NotEngine.Test;

class Sample
{
    internal static class ImageHelper
    {
        public static ImageResult LoadImage(string imgPath)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            return ImageResult.FromMemory(File.ReadAllBytes(imgPath), ColorComponents.RedGreenBlueAlpha);
        }
    }
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
            DepthBits = ContextDepthBits.Depth24,
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
        float[] vertices =
        {
            //     ---- 位置 ----       ---- 颜色 ----     - 纹理坐标 -
            0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f,   // 右上
            0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // 右下
            -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f,   // 左下
            -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f    // 左上
        };
        uint[] indices =
        {
            // note that we start from 0!
            0, 1, 3, // first Triangle
            1, 2, 3 // second Triangle
        };

        string shaderSource = File.ReadAllText("Test.glsl");
        VertexArray vao = new VertexArray();
        VertexBuffer vbo = new VertexBuffer(vertices, new VertexBufferLayout { Count = 3 },
            new VertexBufferLayout { Count = 3,AttributeLocation = 1}, new VertexBufferLayout { Count = 2, AttributeLocation = 2 });
        IndexBuffer ibo = new IndexBuffer(indices);
        vao.BindIndexBuffer(ibo);
        vao.BindVertexBuffer(vbo);
        ShaderProgram shader = new ShaderProgram(shaderSource);
        UniformBuffer ubo = new UniformBuffer(sizeof(float) * 4,1);
        ShaderStorageBuffer ssbo = new ShaderStorageBuffer(1);

        var imageResult = ImageHelper.LoadImage($@"C:\Users\16018\Desktop\Snipaste_2024-10-02_16-55-02.png");
        GraphicTexture texture = new GraphicTexture2D(imageResult.Width, imageResult.Height,false,false);
        texture.UpLoad(0, 0, imageResult.Width, imageResult.Width, imageResult.Height,1, imageResult.Data);

        float[] verticesQuad =
        {
            //     ---- 位置 ---
            1.0f,  1.0f, 0.0f,  1.0f, 1.0f,   // 右上
            1.0f, -1.0f, 0.0f,  1.0f, 0.0f,   // 右下
            -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,   // 左下
            -1.0f,  1.0f, 0.0f, 0.0f, 1.0f    // 左上
        };
        uint[] indicesQuad =
        {
            // note that we start from 0!
            0, 1, 3, // first Triangle
            1, 2, 3 // second Triangle
        };
        VertexArray vao1 = new VertexArray();
        VertexBuffer vbo1 = new VertexBuffer(verticesQuad, new VertexBufferLayout { Count = 3 }, 
            new VertexBufferLayout { Count = 2,AttributeLocation = 1});
        IndexBuffer ibo1 = new IndexBuffer(indicesQuad);
        vao1.BindIndexBuffer(ibo1);
        vao1.BindVertexBuffer(vbo1);
        string shaderSource1 = File.ReadAllText("Quad.glsl");
        ShaderProgram shader1 = new ShaderProgram(shaderSource1);


        
        while (true)
        {
            frameBuffer?.Bind();
            // This will process events for all windows and
            // post those events to the event queue.
            Toolkit.Window.ProcessEvents(false);
            if (Toolkit.Window.IsWindowDestroyed(window))
            {
                break;
            }

            GL.ClearColor(new Color4<Rgba>(64 / 255f, 0, 127 / 255f, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);
            vao.Bind();
            shader.Bind();
            shader.SetTextureHandle("yourTexture",texture.TextureHandleId);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            Toolkit.OpenGL.SwapBuffers(glContext);

            frameBuffer?.Unbind();
            vao1.Bind();
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

        if (args is WindowResizeEventArgs resizeEventArgs)
        {
            var m = resizeEventArgs.NewSize;
            GL.Viewport(0, 0, resizeEventArgs.NewClientSize.X, resizeEventArgs.NewClientSize.Y);
            frameBuffer = new FrameBuffer(resizeEventArgs.NewClientSize.X, resizeEventArgs.NewClientSize.Y, 1);
        }
    }
}