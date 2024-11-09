using MessagePack;
using NotEngine.Rendering;

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
        ShaderProgram = Graphics.Device.CreateShaderProgram(source);
    }
    public Shader(string source)
    {
        Source = source;
        ShaderProgram = Graphics.Device.CreateShaderProgram(source);
        AssetId = Guid.NewGuid();
    }
    public void Dispose()
    {
        ShaderProgram.Dispose();
    }
}