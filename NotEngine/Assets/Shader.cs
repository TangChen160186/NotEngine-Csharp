using MessagePack;
using NotEngine.Graphics;
namespace NotEngine.Assets;
[MessagePackObject(keyAsPropertyName:true)]
public class Shader : IAsset
{
    [IgnoreMember]
    public ShaderProgram ShaderProgram { get; }
    public Guid AssetId { get; }
    public string Source { get; }

    [SerializationConstructor]
    public Shader(Guid assetId, string source)
    {
        AssetId = assetId;
        ShaderProgram = new ShaderProgram(source);
        AssetId = Guid.NewGuid();
    }
    public Shader(string shaderSource)
    {
        Source = shaderSource;
        ShaderProgram = new ShaderProgram(shaderSource);
        AssetId = Guid.NewGuid();
    }
    public void Dispose()
    {
        ShaderProgram.Dispose();
    }
}