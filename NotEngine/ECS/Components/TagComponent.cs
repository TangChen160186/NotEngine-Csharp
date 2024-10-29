using MessagePack;

namespace NotEngine.ECS.Components;

[MessagePackObject(keyAsPropertyName: true)]
public partial class TagComponent: Component
{
    public string Tag { get; set; } = "";
}
