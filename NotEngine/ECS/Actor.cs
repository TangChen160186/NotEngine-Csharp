using MessagePack;
using NotEngine.ECS.Components;

namespace NotEngine.ECS;

[MessagePackObject(keyAsPropertyName:true,AllowPrivate = true)]
public partial class Actor : IDisposable
{
    [IgnoreMember]
    public readonly Scene Scene;
    public List<Component> Components { get;  set; } = [];

    [IgnoreMember]
    private Actor? _parent;

    public Guid ParentId { get; private set; }

    [IgnoreMember]
    public Actor? Parent
    {
        get=> _parent;
        set => SetParent(value);
    }
    [IgnoreMember]
    public List<Actor> Children { get; } = [];

    [SerializationConstructor]
    private Actor() { }
    internal Actor(Scene scene) { Scene = scene;}

    public T AddComponent<T>() where T : Component, new()
    {
        T component = new T
        {
            Actor = this
        };
        Components.Add(component);
        return component;
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
    public void SetParent(Actor? newParent)
    {

        if (_parent == newParent || newParent == this) return;

        _parent?.Children.Remove(this);

        _parent = newParent;
        newParent?.Children.Add(this);
        ParentId = newParent?.GetComponent<IdComponent>()?.Id ?? Guid.Empty;
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