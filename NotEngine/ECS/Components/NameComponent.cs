using MessagePack;

namespace NotEngine.ECS.Components;

[MessagePackObject(keyAsPropertyName: true)]
public partial class NameComponent: Component
{
    public string Name { get; set; } = "New Actor";
}