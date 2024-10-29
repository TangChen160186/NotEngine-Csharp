using NotEngine.ECS.Components;

namespace NotEngine.ECS.Systems;

public interface ISystem
{
    void Run();
}

public class RenderSystem : ISystem
{
    private Scene _scene;

    
    public RenderSystem(Scene scene)
    {
        _scene = scene;
    }
    public void Run()
    {
        List<(MeshFilterComponent, MeshRenderComponent)> renderComponent =
            new List<(MeshFilterComponent, MeshRenderComponent)>();

        foreach (var actor in _scene.Actors)
        {
            if (actor.HasComponent<MeshFilterComponent>() && actor.HasComponent<MeshRenderComponent>())
            {
                renderComponent.Add((actor.GetComponent<MeshFilterComponent>(),actor.GetComponent<MeshRenderComponent>())!);
            }
        }

        foreach (var (meshFilterComponent, meshRenderComponent) in renderComponent)
        {
            meshFilterComponent.Mesh.Asset.Bind();


        }
    }
}



