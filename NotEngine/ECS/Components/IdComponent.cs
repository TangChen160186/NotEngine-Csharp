using MessagePack;

namespace NotEngine.ECS.Components;

[MessagePackObject(keyAsPropertyName: true)]
public partial class IdComponent: Component
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}