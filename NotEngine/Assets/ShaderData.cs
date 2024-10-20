//using NotEngine.Core;
//using Veldrid;
//using Veldrid.SPIRV;

//namespace NotEngine.Assets;

//public class ShaderData : IAsset
//{
//    public bool IsDisposed { get; private set; }
//    public Guid AssetId { get; }
//    public EAssetType Type => EAssetType.Shader;
//    public int RefCount { get; set; }

//    private readonly GraphicsDevice _graphicsDevice;
//    public Shader[] Shaders { get; private set; }
//    public byte[] VertexSourceBytes { get; }
//    public byte[] FragmentSourceBytes { get; }

//    public ShaderData(byte[] vertexSourceBytes, byte[] fragmentSourceBytes)
//    {
//        AssetId = Guid.NewGuid();
//        _graphicsDevice = Application.Current.Device;
//        VertexSourceBytes = vertexSourceBytes;
//        FragmentSourceBytes = fragmentSourceBytes;
//    }
//    public ShaderData(byte[] vertexSourceBytes, byte[] fragmentSourceBytes,Guid guid)
//    {
//        AssetId = guid;
//        _graphicsDevice = Application.Current.Device;
//        VertexSourceBytes = vertexSourceBytes;
//        FragmentSourceBytes = fragmentSourceBytes;
//    }
//    public void UpLoad()
//    {
//        Shaders = new Shader[2];
//        ShaderDescription vertexShaderDesc = new ShaderDescription(
//            ShaderStages.Vertex,
//            VertexSourceBytes,
//            "main");
//        ShaderDescription fragmentShaderDesc = new ShaderDescription(
//            ShaderStages.Fragment,
//            FragmentSourceBytes,
//            "main");

//        Shaders = 
//            _graphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

//    }

//    public void Dispose()
//    {
//        Shaders[0].Dispose();
//        Shaders[1].Dispose();
//        IsDisposed = true;
//    }
//}