using MessagePack;
using NotEngine.ECS.Components;

namespace NotEngine.ECS;
[MessagePackObject(keyAsPropertyName:true)]
public partial class Scene: IMessagePackSerializationCallbackReceiver
{
    [IgnoreMember]
    public int Width { get; private set; }
    [IgnoreMember]
    public int Height { get; private set; }

    public void UpdateSceneSize(int width,int height)
    {
        Width = width;
        Height = height;
    }

    public List<Actor> Actors { get;private set; } = [];
    public Scene()
    {
       
    }
    public Actor CreateActor()
    {
        var actor = new Actor(this);
        actor.AddComponent<ActiveComponent>();
        actor.AddComponent<IdComponent>();
        actor.AddComponent<NameComponent>();
        actor.AddComponent<TagComponent>();
        actor.AddComponent<TransformComponent>();
        Actors.Add(actor);
        return actor;
    }

    public Actor? FindActorById(Guid id)
    {
        return Actors.FirstOrDefault(e => 
            e.GetComponent<IdComponent>()!.Id == id);
    }
    public List<Actor> FindActorByTag(string tag)
    {
        return Actors.Where(e =>
            e.GetComponent<TagComponent>()!.Tag == tag).ToList();
    }
    public List<Actor> FindActorByName(string name)
    {
        return Actors.Where(e =>
            e.GetComponent<NameComponent>()!.Name == name).ToList();
    }

    public bool RemoveActor(Actor actor)
    {

        return Actors.Remove(actor);
    }

    public List<Actor> GetActorsWithComponent<T>() where T : Component
    {
        return Actors.Where(e => e.HasComponent<T>()).ToList();
    }


    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        foreach (var actor in Actors.Where(actor => actor.ParentId != Guid.Empty))
        {
            actor.Parent = FindActorById(actor.ParentId);
        }
    }
}