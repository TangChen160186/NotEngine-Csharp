using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace NotEngine.Test
{
    class Program
    {
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


        static void Main(string[] args)
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Veldrid Tutorial"
            };
            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            };

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options, GraphicsBackend.OpenGLES);

            #region Test


            #endregion


            CreateResources();

            while (window.Exists)
            {
                window.PumpEvents();

                if (window.Exists)
                {
                    Draw();
                }
            }

            DisposeResources();
        }

        private static DeviceBuffer _uBuffer;
        private static ResourceSet _resourceSet;
        private static void CreateResources()
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

            ushort[] quadIndices = { 0, 1, 2, 3 };
            BufferDescription ibDescription = new BufferDescription(
                4 * sizeof(ushort),
                BufferUsage.IndexBuffer);
            _indexBuffer = factory.CreateBuffer(ibDescription);
            _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(VertexCode),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(FragmentCode),
                "main");
     
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
            pipelineDescription.ResourceLayouts = [factory.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription("UniformBufferObject", ResourceKind.UniformBuffer,ShaderStages.Vertex)))];
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);
            pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;
            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            _commandList = factory.CreateCommandList();
            _resourceSet = factory.CreateResourceSet(new ResourceSetDescription(pipelineDescription.ResourceLayouts[0], _uBuffer));
        }

        private static void Draw()
        {
            Vector4 color = new Vector4(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1);
      
            // Begin() must be called before commands can be issued.
            _commandList.Begin();
            _graphicsDevice.UpdateBuffer(_uBuffer, 0, ref color, sizeof(float) * 4);
            // We want to render directly to the output window.
            _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);

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
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);

            // Once commands have been submitted, the rendered image can be presented to the application window.
            _graphicsDevice.SwapBuffers();
        }

        private static void DisposeResources()
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

    struct VertexPositionColor
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
}