using NotEngine.ECS.Components;

namespace NotEngine.ECS.Systems;

public sealed class TransformSystem(Scene scene) : ISystem
{
    public void Run()
    {
        var rootActors = GetRootActors();
        foreach (var actor in rootActors)
        {
            UpdateTransform(actor);
        }
    }

    private void UpdateTransform(Actor actor)
    {
        foreach (var child in actor.Children)
        {
            child.GetComponent<TransformComponent>()!.SetParentTransformMatrix(actor.GetComponent<TransformComponent>()!
                .WorldTransform);
            UpdateTransform(child);
        }
    }

    private List<Actor> GetRootActors()
    {
        return scene.Actors.Where(e => e.Parent == null).ToList();
    }
}