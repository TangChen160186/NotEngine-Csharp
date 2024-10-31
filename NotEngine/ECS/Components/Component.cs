using MessagePack;
using NotEngine.Assets;

namespace NotEngine.ECS.Components;
[MessagePack.Union(0, typeof(ActiveComponent))]
[MessagePack.Union(1, typeof(CameraComponent))]
[MessagePack.Union(2, typeof(IdComponent))]
[MessagePack.Union(3, typeof(MeshFilterComponent))]
[MessagePack.Union(4, typeof(MeshRenderComponent))]
[MessagePack.Union(5, typeof(NameComponent))]
[MessagePack.Union(6, typeof(TagComponent))]
[MessagePack.Union(7, typeof(TransformComponent))]
public abstract class Component: IDisposable
{
    [IgnoreMember]
    public Actor Actor { get; internal set; }

    public virtual void Dispose()
    {
        Actor.RemoveComponent(this);
    }
}


