namespace NotEngine.Assets;
[MessagePack.Union(0, typeof(Texture))]
[MessagePack.Union(1, typeof(Mesh))]
[MessagePack.Union(2, typeof(Shader))]
[MessagePack.Union(3, typeof(Material))]
public interface IAsset : IDisposable
{
    public Guid AssetId { get;}
}