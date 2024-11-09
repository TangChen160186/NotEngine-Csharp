using System.Numerics;
using MessagePack;
using NotEngine.Rendering;
using OpenTK.Graphics.OpenGL;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true)]
public partial class Material : IAsset
{
    public Guid AssetId { get; }
    public RasterizerState RasterizerState { get; set; } = new();
    [IgnoreMember]
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
                else if (uniformInfo.Type == EUniformType.Texture2D)
                {
                    if(uniformInfo.Value!=null)
                        Shader.Asset.ShaderProgram.SetTextureHandle(uniformInfo.Name, ((AssetRef<Texture2D>)uniformInfo.Value).Asset!.TextureHandleId);
                }
            }
        }

    }

    public void SetUniform(string name,object? data)
    {
        if(Uniforms==null) return;
        if (Uniforms.Exists(e => e.Name == name))
        {
            var uniformInfo = Uniforms.First(e => e.Name == name);
            uniformInfo!.Value = data;
        }
    }

}