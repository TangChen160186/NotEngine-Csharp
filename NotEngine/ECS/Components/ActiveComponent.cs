using MessagePack;

namespace NotEngine.ECS.Components;

[MessagePackObject(keyAsPropertyName: true)]
public class ActiveComponent: Component
{
    public bool SelfActive { get; set; } = true;

    [IgnoreMember] public bool Active =>Actor.Parent==null?SelfActive: Actor.Parent.GetComponent<ActiveComponent>()!.Active && SelfActive;
}