using System.ComponentModel.Composition;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Gemini.Framework;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using WindowState = Veldrid.WindowState;

namespace Module.Scene.ViewModels;

[Export]
public class SceneViewModel : Document
{
    public override string DisplayName { get; set; } = "Scene";
    private WriteableBitmap _drawBitmap;

    public WriteableBitmap DrawBitmap
    {
        get => _drawBitmap;
        set
        {
            _drawBitmap = value;
            Set(ref _drawBitmap, value);
        }
    }

    private static GraphicsDevice _graphicsDevice;
    private static CommandList _commandList;
    private static DeviceBuffer _vertexBuffer;
    private static DeviceBuffer _indexBuffer;
    private static Shader[] _shaders;
    private static Pipeline _pipeline;

    private const string VertexCode = @"
    #version 450

    layout(location = 0) in vec2 Position;
    layout(location = 1) in vec4 Color;

    layout(location = 0) out vec4 fsin_Color;
    layout(binding = 0) uniform UniformBufferObject {
        vec4 uColor;
    } ubo;
    void main()
    {
        gl_Position = vec4(Position, 0, 1);
        fsin_Color = ubo.uColor;
    }";

    private const string FragmentCode = @"
    #version 450

    layout(location = 0) in vec4 fsin_Color;
    layout(location = 0) out vec4 fsout_Color;

    void main()
    {
    fsout_Color = fsin_Color;
    }";


    private static DeviceBuffer _uBuffer;
    private static ResourceSet _resourceSet;
    private static Framebuffer _framebuffer;

    private void CreateResources()
    {
        ResourceFactory factory = _graphicsDevice.ResourceFactory;
        _uBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(float), BufferUsage.UniformBuffer));

        VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
            new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
            new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
            new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
        };
        BufferDescription vbDescription = new BufferDescription(
            4 * VertexPositionColor.SizeInBytes,
            BufferUsage.VertexBuffer);
        _vertexBuffer = factory.CreateBuffer(vbDescription);
        _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);

        ushort[] quadIndices = [0, 1, 2, 3];
        BufferDescription ibDescription = new BufferDescription(
            4 * sizeof(ushort),
            BufferUsage.IndexBuffer);
        _indexBuffer = factory.CreateBuffer(ibDescription);
        _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

        VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                VertexElementFormat.Float2),
            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

        ShaderDescription vertexShaderDesc = new ShaderDescription(
            ShaderStages.Vertex,
            Encoding.UTF8.GetBytes(VertexCode),
            "main");
        ShaderDescription fragmentShaderDesc = new ShaderDescription(
            ShaderStages.Fragment,
            Encoding.UTF8.GetBytes(FragmentCode),
            "main");
        var reflectCompilationResult = SpirvCompilation.CompileVertexFragment(Encoding.UTF8.GetBytes(VertexCode),
            Encoding.UTF8.GetBytes(FragmentCode), CrossCompileTarget.GLSL, new CrossCompileOptions());
        var vertexElementDescription = reflectCompilationResult.Reflection.VertexElements;
        var resourceLayoutDescription = reflectCompilationResult.Reflection.ResourceLayouts;
        _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

        // Create pipeline
        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual);
        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: FaceCullMode.Back,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false);

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
        pipelineDescription.ResourceLayouts =
        [
            factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("UniformBufferObject", ResourceKind.UniformBuffer,
                    ShaderStages.Vertex)))
        ];
        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
            shaders: _shaders);
        pipelineDescription.Outputs = _framebuffer.OutputDescription;
        _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        _commandList = factory.CreateCommandList();
        _resourceSet =
            factory.CreateResourceSet(new ResourceSetDescription(pipelineDescription.ResourceLayouts[0], _uBuffer));
    }

    public static Sdl2Window window;

    public SceneViewModel()
    {
        WindowCreateInfo windowCI = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 984,
            WindowHeight = 705,
            WindowTitle = "Veldrid Tutorial",
            WindowInitialState = WindowState.Hidden
        };
        window = VeldridStartup.CreateWindow(ref windowCI);

        GraphicsDeviceOptions options = new GraphicsDeviceOptions
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true
        };

        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options, GraphicsBackend.OpenGLES);
    }

    private Texture targetTexture;

    private UserControl userControl;


    protected override void OnViewReady(object view)
    {
        base.OnViewReady(view);
        //userControl = (UserControl)view;
        //_drawBitmap = new WriteableBitmap((int)userControl.ActualWidth,
        //    (int)userControl.ActualHeight, 96, 96, PixelFormats.Rgba64, null);
        //TextureDescription mainColorDesc = TextureDescription.Texture2D(
        //    (uint)userControl.ActualWidth,
        //    (uint)userControl.ActualHeight,
        //    1,
        //    1,
        //    PixelFormat.R16_G16_B16_A16_UNorm,
        //    TextureUsage.RenderTarget | TextureUsage.Sampled,
        //    TextureSampleCount.Count1);
        //Texture mainSceneColorTexture = _graphicsDevice.ResourceFactory.CreateTexture(ref mainColorDesc);
        //mainColorDesc.Usage = TextureUsage.Staging;
        //targetTexture = _graphicsDevice.ResourceFactory.CreateTexture(ref mainColorDesc);
        //_framebuffer = _graphicsDevice.ResourceFactory.CreateFramebuffer(
        //    new FramebufferDescription(null, mainSceneColorTexture));

        //CreateResources();
        //_drawBitmap = new WriteableBitmap((int)userControl.ActualWidth,
        //    (int)userControl.ActualHeight, 96, 96, PixelFormats.Rgba64, null);

        //// 设置定时器

        //_timer = new DispatcherTimer(DispatcherPriority.Render);
        //_timer.Interval = TimeSpan.FromMilliseconds(Interval);
        //_timer.Tick += OnTimerTick; // 定时执行绘制操作
        //_timer.Start();
    }

    private DispatcherTimer _timer;
    private const double TargetFps = 10;
    private const double Interval = 1000 / TargetFps; // 每帧间隔16.67ms

    private void OnTimerTick(object sender, EventArgs e)
    {
        // 在这里执行每一帧的绘制逻辑
        Draw();
    }


    private async void Draw()
    {
        Vector4 color = new Vector4(1, 1, 0, 1);
        _graphicsDevice.UpdateBuffer(_uBuffer, 0, ref color, sizeof(float) * 4);
        // Begin() must be called before commands can be issued.
        _commandList.Begin();

        // We want to render directly to the output window.
        _commandList.SetFramebuffer(_framebuffer);
        _commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);

        // Set all relevant state to draw our quad.
        _commandList.SetVertexBuffer(0, _vertexBuffer);
        _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        _commandList.SetPipeline(_pipeline);
        _commandList.SetGraphicsResourceSet(0, _resourceSet);
        // Issue a Draw command for a single instance with 4 indices.
        _commandList.DrawIndexed(
            indexCount: 4,
            instanceCount: 1,
            indexStart: 0,
            vertexOffset: 0,
            instanceStart: 0);

        // End() must be called before commands can be submitted for execution.


        _commandList.CopyTexture(_framebuffer.ColorTargets[0].Target, targetTexture, 0, 0);
        _commandList.End();
        _graphicsDevice.SubmitCommands(_commandList);

        // Once commands have been submitted, the rendered image can be presented to the application window.
        //_graphicsDevice.SwapBuffers();

        var source = _graphicsDevice.Map(targetTexture, MapMode.Read);
        WritePixelsToBitmap(DrawBitmap, source.Data, (int)userControl.ActualWidth, (int)userControl.ActualHeight);
        _graphicsDevice.Unmap(targetTexture);
    }

    // 写像素数据到 WriteableBitmap，使用非托管指针
    static void WritePixelsToBitmap(WriteableBitmap bitmap, nint pixelPointer, int width, int height)
    {
        // 锁定 WriteableBitmap 的像素缓冲区
        bitmap.Lock();
        // 计算每行的字节数（Rgba64 每个像素 8 字节）
        int stride = width * 8;

        // 使用指针数据写入 WriteableBitmap
        bitmap.WritePixels(
            new Int32Rect(0, 0, width, height), // 目标区域
            pixelPointer,
            stride * height // 源数据的指针
            , // 每行字节数
            stride // 数据偏移量
        );
        bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));

        // 解锁 WriteableBitmap
        bitmap.Unlock();
    }

    private void DisposeResources()
    {
        _pipeline.Dispose();
        foreach (Shader shader in _shaders)
        {
            shader.Dispose();
        }

        _commandList.Dispose();
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _graphicsDevice.Dispose();
    }
}

public struct VertexPositionColor
{
    public const uint SizeInBytes = 24;
    public Vector2 Position;
    public RgbaFloat Color;

    public VertexPositionColor(Vector2 position, RgbaFloat color)
    {
        Position = position;
        Color = color;
    }
}