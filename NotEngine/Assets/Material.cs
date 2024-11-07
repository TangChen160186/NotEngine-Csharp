using System.Numerics;
using MessagePack;
using NotEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true)]
public partial class Material : IAsset
{
    public Guid AssetId { get; }
    [IgnoreMember]


    #region 基础设置

    public bool AlphaToMask { get; set; } = false;
    public bool Blendable { get; set; } = false;
    public BlendingFactor SourceBlend { get; set; } = BlendingFactor.One;
    public BlendingFactor DestinationBlend { get; set; } = BlendingFactor.Zero;
    public BlendEquationMode BlendOp { get; set; } = BlendEquationMode.FuncAdd;

    public bool ColorWriteR { get; set; } = true;
    public bool ColorWriteG { get; set; } = true;
    public bool ColorWriteB { get; set; } = true;
    public bool ColorWriteA { get; set; } = true;

    public TriangleFace Cull { get; set; } = TriangleFace.Back;

    public bool DepthTest { get; set; } = true;
    public DepthFunction ZTest { get; set; } = DepthFunction.Lequal;
    public bool ZWrite { get; set; } = true;
    public bool ZClip { get; set; } = true;

    public bool DepthWriting { get; set; } = true;

    public float OffsetFactor { get; set; } = 0.0f;
    public float OffsetUnits { get; set; } = 0.0f;

    public StencilFunction StencilFunc { get; set; } = StencilFunction.Always;
    public int StencilRef { get; set; } = 0;
    public uint StencilMask { get; set; } = 0xFF;
    public StencilOp StencilFail { get; set; } = StencilOp.Keep;
    public StencilOp StencilZFail { get; set; } = StencilOp.Keep;
    public StencilOp StencilPass { get; set; } = StencilOp.Keep;

    #endregion
    public List<UniformInfo>? Uniforms { get; private set; }

    [IgnoreMember]
    private AssetRef<Shader> _shader;

    public AssetRef<Shader> Shader
    {
        get => _shader;
        set => SetShader(value.Asset);
    }

    public void SetShader(Shader? shader)
    {
        Shader.Dispose();
        _shader = shader;
        if (_shader.HasValue)
        {
            Uniforms = _shader.Asset!.ShaderProgram.QueryUniforms();
        }
    }

    [SerializationConstructor]
    private Material(Guid assetId, AssetRef<Shader> shader, List<UniformInfo>? uniforms)
    {
        AssetId = assetId;
        Uniforms = uniforms;
        Shader = shader;
    }

    public Material(Shader? shader = null)
    {
        AssetId = Guid.NewGuid();
        Shader = shader;
    }

    public void Dispose()
    {
        Shader.Dispose();
    }

    public void Apply()
    {
        Shader.Asset?.ShaderProgram.Bind();
        // Alpha to Coverage
        if (AlphaToMask)
            GL.Enable(EnableCap.SampleAlphaToCoverage);
        else
            GL.Disable(EnableCap.SampleAlphaToCoverage);

        // Blending
        if (Blendable)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(SourceBlend, DestinationBlend);
            GL.BlendEquation(BlendOp);
        }
        else
        {
            GL.Disable(EnableCap.Blend);
        }
        // Color Mask
        GL.ColorMask(ColorWriteR, ColorWriteG, ColorWriteB, ColorWriteA);
 

        // Culling
        GL.CullFace(Cull);

        // Depth Test
        if (DepthTest)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(ZTest);
        }
        else
        {
            GL.Disable(EnableCap.DepthTest);
        }

        // Depth Writing
        GL.DepthMask(ZWrite);

        // Depth Clipping
        if (ZClip)
            GL.Enable(EnableCap.DepthClamp);
        else
            GL.Disable(EnableCap.DepthClamp);

        // Polygon Offset
        if (OffsetFactor != 0.0f || OffsetUnits != 0.0f)
        {
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(OffsetFactor, OffsetUnits);
        }
        else
        {
            GL.Disable(EnableCap.PolygonOffsetFill);
        }

        // Stencil
        GL.Enable(EnableCap.StencilTest);
        GL.StencilFunc(StencilFunc, StencilRef, StencilMask);
        GL.StencilOp(StencilFail, StencilZFail, StencilPass);


        if (Shader.Asset != null && Uniforms!=null)
        {
            foreach (var uniformInfo in Uniforms)
            {

                if (uniformInfo.Type == EUniformType.Bool)
                {
                    Shader.Asset.ShaderProgram.SetUniform(uniformInfo.Name,(bool)uniformInfo.Value!);
                }
                else if (uniformInfo.Type == EUniformType.Int)
                {
                    Shader.Asset.ShaderProgram.SetUniform(uniformInfo.Name, (int)uniformInfo.Value!);
                }
                else if (uniformInfo.Type == EUniformType.Float)
                {
                    Shader.Asset.ShaderProgram.SetUniform(uniformInfo.Name, (float)uniformInfo.Value!);
                }
                else if (uniformInfo.Type == EUniformType.V2)
                {
                    Shader.Asset.ShaderProgram.SetUniform(uniformInfo.Name, (Vector2)uniformInfo.Value!);
                }
                else if (uniformInfo.Type == EUniformType.V3)
                {
                    Shader.Asset.ShaderProgram.SetUniform(uniformInfo.Name, (Vector3)uniformInfo.Value!);
                }
                else if (uniformInfo.Type == EUniformType.V4)
                {
                    Shader.Asset.ShaderProgram.SetUniform(uniformInfo.Name, (Vector4)uniformInfo.Value!);
                }
                else if (uniformInfo.Type == EUniformType.Texture)
                {
                    if(uniformInfo.Value!=null)
                        Shader.Asset.ShaderProgram.SetTextureHandle(uniformInfo.Name, ((AssetRef<Texture>)uniformInfo.Value).Asset!.TextureHandleId);
                }
            }
        }

    }
}