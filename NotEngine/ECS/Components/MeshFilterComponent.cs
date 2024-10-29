using MessagePack;
using NotEngine.Assets;

namespace NotEngine.ECS.Components;

[MessagePackObject(keyAsPropertyName: true)]
public partial class MeshFilterComponent:Component,IDisposable
{
    public AssetRef<Mesh> Mesh{ get; set; }
    public bool Enable { get; set; } = true;
    public override void Dispose()
    {
        Mesh.Dispose();
        base.Dispose();
    }
}

