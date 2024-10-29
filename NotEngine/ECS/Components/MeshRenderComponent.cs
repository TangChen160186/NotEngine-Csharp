using MessagePack;
using NotEngine.Assets;

namespace NotEngine.ECS.Components;

[MessagePackObject(keyAsPropertyName:true)]
public partial class MeshRenderComponent: Component
{
    public bool Enable { get; set; }
    public AssetRef<Material> Material { get; set; }

    public void SetMaterial(Material? material = null)
    {
        Material = new AssetRef<Material>(material);
    }

    public override void Dispose()
    {
        Material.Dispose();
        base.Dispose();
    }
}