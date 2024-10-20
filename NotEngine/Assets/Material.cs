//using NotEngine.Core;
//using Veldrid;
//using Veldrid.SPIRV;
//namespace NotEngine.Assets;

//public class Material: IAsset
//{
//    public Guid AssetId { get; }
//    public EAssetType Type => EAssetType.Material;
//    public int RefCount { get; set; }
//    internal Pipeline Pipleline { get; private set; }

//    #region Rasterizer

//    public bool Blendable { get; set; }

//    public bool DepthTest { get; set; }
//    public bool DepthWriting { get; set; }


//    public bool BackFaceCulling { get; set; }
//    public bool FontFaceCulling { get; set; }


//    public bool ColorWriting { get; set; }
//    public int GpuInstances { get; set; }

//    #endregion

//    public ShaderData ShaderData { get; set; }

//    private GraphicsDevice _graphicsDevice;


    
//    public Material(ShaderData shaderData, Framebuffer? frameBuffer = null)
//    {
//        AssetId = Guid.NewGuid();
//        _graphicsDevice = Application.Current.Device;
//        ShaderData = shaderData;
//        CreatePipeline(shaderData, frameBuffer);
//    }

//    public Material(ShaderData shaderData,Guid guid)
//    {
//        AssetId = guid;
//        _graphicsDevice = Application.Current.Device;
//        ShaderData = shaderData;
//        CreatePipeline(shaderData);
//    }
//    private void CreatePipeline(ShaderData shaderData,Framebuffer? framebuffer = null)
//    {
//        var reflectCompilationResult = SpirvCompilation.CompileVertexFragment(ShaderData.VertexSourceBytes,
//            shaderData.FragmentSourceBytes, CrossCompileTarget.GLSL, new CrossCompileOptions());
//        var vertexElementDescription = reflectCompilationResult.Reflection.VertexElements;
//        var resourceLayoutDescription = reflectCompilationResult.Reflection.ResourceLayouts;
      

//        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
//        pipelineDescription.BlendState = Blendable ? BlendStateDescription.SingleDisabled : BlendStateDescription.SingleAlphaBlend;
//        if (!DepthTest)
//        {
//            pipelineDescription.DepthStencilState = DepthStencilStateDescription.Disabled;
//        }
//        else
//        {
//            pipelineDescription.DepthStencilState = DepthWriting ? DepthStencilStateDescription.DepthOnlyLessEqual :
//                DepthStencilStateDescription.DepthOnlyLessEqualRead;
//        }

//        RasterizerStateDescription rasterizerStateDescription;
//        if (!BackFaceCulling && !FontFaceCulling)
//        {
//            rasterizerStateDescription = RasterizerStateDescription.CullNone;
//        }
//        else
//        {
//            rasterizerStateDescription = RasterizerStateDescription.Default;
//            if (BackFaceCulling && FontFaceCulling)
//            {
//                rasterizerStateDescription.CullMode = FaceCullMode.Back | FaceCullMode.Front;
//            }
//            else if (BackFaceCulling)
//            {
//                rasterizerStateDescription.CullMode = FaceCullMode.Back;
//            }
//            else
//            {
//                rasterizerStateDescription.CullMode = FaceCullMode.Front;
//            }
//        }

//        pipelineDescription.RasterizerState = rasterizerStateDescription;
//        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;

//        VertexLayoutDescription vertexLayoutDescription = new VertexLayoutDescription(vertexElementDescription);
//        pipelineDescription.ShaderSet = new ShaderSetDescription([vertexLayoutDescription], shaderData.Shaders);


//        ResourceLayout[] resourceLayouts = new ResourceLayout[resourceLayoutDescription.Length];
//        for (int i = 0; i < resourceLayoutDescription.Length; i++)
//        {
//            resourceLayouts[i] =
//                _graphicsDevice.ResourceFactory.CreateResourceLayout(resourceLayoutDescription[i]);
//        }
//        pipelineDescription.ResourceLayouts = resourceLayouts;
//        if (framebuffer != null)
//        {
//            pipelineDescription.Outputs = framebuffer.OutputDescription;
//        }
//        else
//        {
//            pipelineDescription.Outputs = new OutputDescription(
//                new OutputAttachmentDescription(TextureFormat.D32_Float_S8_UInt),
//                new OutputAttachmentDescription(TextureFormat.R8_G8_B8_A8_UNorm));
//        }        
//        Pipleline = _graphicsDevice.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
//    }
//    public void Dispose()
//    {
//        Pipleline.Dispose();
//    }
//} 