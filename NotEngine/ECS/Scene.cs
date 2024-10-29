namespace NotEngine.ECS;

public class Scene
{
    public List<Actor> Actors = [];
    public Scene()
    {
       
    }

    public Actor CreateActor()
    {
        var actor = new Actor(this);
        Actors.Add(actor);
        return actor;
    }

    public bool RemoveActor(Actor actor)
    {
        return Actors.Remove(actor);
    }
}