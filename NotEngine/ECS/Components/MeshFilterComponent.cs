using NotEngine.Assets;

namespace NotEngine.ECS.Components;

public struct MeshFilterComponent : IEntityComponent
{
    public Mesh? Mesh { get; set; }
}

