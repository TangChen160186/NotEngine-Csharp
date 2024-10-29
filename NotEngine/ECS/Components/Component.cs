using MessagePack;

namespace NotEngine.ECS.Components;

public abstract class Component: IDisposable
{
    [IgnoreMember]
    public Actor Actor { get; internal set; }

    public virtual void Dispose()
    {
        Actor.RemoveComponent(this);
    }
}


