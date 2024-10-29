using NotEngine.ECS.Components;

namespace NotEngine.ECS;

public class Actor : IDisposable
{
    public readonly Scene Scene;
    public List<Component> Components { get; } = [];

    internal Actor(Scene scene)
    {
        Scene = scene;
    }

    public void AddComponent<T>() where T : Component, new()
    {
        T component = new T
        {
            Actor = this
        };
        Components.Add(component);
        
    }

    public bool HasComponent<T>() where T : Component
    {
        return Components.OfType<T>().Any();
    }

    public T? GetComponent<T>() where T : Component
    {
        return Components.OfType<T>().FirstOrDefault();
    }

    public List<T> GetComponents<T>() where T : Component
    {
        return Components.OfType<T>().ToList();
    }

    public T GetOrAddComponent<T>() where T : Component, new()
    {
        var component = GetComponent<T>();
        if (component == null)
        {
            component = new T()
            {
                Actor = this
            };
            Components.Add(component);
        }
        return component;
    }

    public void RemoveComponents<T>() where T : Component
    {
        var componentsToRemove = GetComponents<T>();
        foreach (var component in componentsToRemove)
        {
            component.Dispose();
            Components.Remove(component);
        }
    }

    public void RemoveComponent(Component component)
    {
        Components.Remove(component);
        component.Dispose();
        
    }
    public void Dispose()
    {
        foreach (var component in Components)
        {
            component.Dispose();
        }

        Scene.RemoveActor(this);
    }
}