using NotEngine.Assets;

namespace NotEngine.ECS.Components;

public struct MeshFilterComponent : IEntityComponent
{
    public StaticMesh? Mesh { get; set; }
}

