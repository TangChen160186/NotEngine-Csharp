using MessagePack;

namespace NotEngine.ECS.Components;

[MessagePackObject(keyAsPropertyName: true)]
public partial class ActiveComponent: Component
{
    public bool SelfActive { get; set; } = true;
}