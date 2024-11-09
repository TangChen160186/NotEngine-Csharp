using System.Numerics;
using NotEngine.ECS.Components;
using NotEngine.Rendering;
using OpenTK.Graphics.OpenGL;

namespace NotEngine.ECS.Systems;

public class RenderSystem(Scene scene) : ISystem
{
    private int _width = scene.Width;
    private int _height = scene.Height;

    //private FrameBuffer _frameBuffer = new FrameBuffer(scene.Width, scene.Height, 1);
    private UniformBuffer _ubo = Graphics.Device.CreateUniformBuffer(1024);


    public unsafe void Run()
    {
        var camera = GetCamera();
        if (camera == null) return;
        

        if (scene.Width != _width || scene.Height != _height)
        {
            //_frameBuffer = new FrameBuffer(scene.Width, scene.Height, 1);
            _width = scene.Width;
            _height = scene.Height;
        }
        //_frameBuffer.Bind();


        List<(MeshFilterComponent, MeshRenderComponent, float,Matrix4x4)> renderTransparentComponent =
            new List<(MeshFilterComponent, MeshRenderComponent, float, Matrix4x4)>();

        List<(MeshFilterComponent, MeshRenderComponent, float, Matrix4x4)> renderOpaqueComponent =
            new List<(MeshFilterComponent, MeshRenderComponent, float, Matrix4x4)>();

        var cameraTransform = camera.GetComponent<TransformComponent>()!;
        foreach (var actor in scene.Actors)
        {
            if(!actor.GetComponent<ActiveComponent>()!.Active) continue;
            var meshFilterComponent = actor.GetComponent<MeshFilterComponent>();
            var meshRenderComponent = actor.GetComponent<MeshRenderComponent>();
            var transform = actor.GetComponent<TransformComponent>()!;
            if (meshFilterComponent != null && meshRenderComponent != null)
            {
                if (meshFilterComponent.Enable && meshRenderComponent.Enable && meshRenderComponent.Material.HasValue && meshFilterComponent.Mesh.HasValue)
                {
                    if (meshRenderComponent.Material.Asset!.RasterizerState.Blendable)
                    {
                        renderTransparentComponent.Add((meshFilterComponent, meshRenderComponent,
                            Vector3.DistanceSquared(cameraTransform.WorldPosition, transform.WorldPosition),transform.WorldTransform));
                    }
                    else
                    {
                        renderOpaqueComponent.Add((meshFilterComponent, meshRenderComponent,
                            Vector3.DistanceSquared(cameraTransform.WorldPosition, transform.WorldPosition), transform.WorldTransform));
                    }
                }
            }
        }

        var vp = GetCameraVp()!;

        Matrix4x4 v = vp.Value.Item1;
        Matrix4x4 p = vp.Value.Item2;
        Vector3 worldPosition = cameraTransform.WorldPosition;
        _ubo.SetSubData((IntPtr)(&v),16*sizeof(float), 16 * sizeof(float));
        _ubo.SetSubData((IntPtr)(&p),16*sizeof(float), 16 * sizeof(float) * 2);
        _ubo.SetSubData((IntPtr)(&worldPosition),3*sizeof(float), 16 * sizeof(float) * 3);

        foreach (var (meshFilterComponent, meshRenderComponent, _,worldTransform) in renderOpaqueComponent.OrderBy(e => e.Item3))
        {
            _ubo.SetSubData((IntPtr)(&worldTransform), 16 * sizeof(float), 0);

            Graphics.DrawMesh(meshFilterComponent.Mesh.Asset, meshRenderComponent.Material.Asset);
        }


        foreach (var (meshFilterComponent, meshRenderComponent, _, worldTransform) in renderTransparentComponent.OrderByDescending(e => e.Item3))
        {
            _ubo.SetSubData((IntPtr)(&worldTransform), 16 * sizeof(float), 0);

            Graphics.DrawMesh(meshFilterComponent.Mesh.Asset, meshRenderComponent.Material.Asset);
        }
    }



    private Actor? GetCamera()
    {
        var actors = scene.GetActorsWithComponent<CameraComponent>();
        return actors.Any() ? actors.Last() : null;
    }

    private (Matrix4x4, Matrix4x4)? GetCameraVp()
    {
        var camera = GetCamera();

        if (camera == null) return null;
        var cameraTf = camera.GetComponent<TransformComponent>()!;
        var cc = camera.GetComponent<CameraComponent>()!;
        return (CalculateViewMatrix(cameraTf.WorldPosition, cameraTf.Rotation),
            CalculateProjectionMatrix(scene.Width, scene.Height, cc));

    }

    private Matrix4x4 CalculateProjectionMatrix(int viewportWidth, int viewportHeight,CameraComponent cc)
    {
        switch (cc.Projection)
        {
            case EProjection.Orthographic:
                return Matrix4x4.CreateOrthographic(cc.Size, cc.Size, cc.Near, cc.Far);
            case EProjection.Perspective:       
                var ratio = viewportWidth / (float)viewportHeight;
                return Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI * cc.Fov / 180f, ratio, cc.Near, cc.Far);
        }

        throw new ArgumentException();
    }
    private Matrix4x4 CalculateViewMatrix(Vector3 position, Quaternion rotation)
    {
        var forward = new OpenTK.Mathematics.Quaternion(rotation.X,rotation.Y,rotation.Z,rotation.W) * OpenTK.Mathematics.Vector3.UnitZ;
        var up = new OpenTK.Mathematics.Quaternion(rotation.X,rotation.Y,rotation.Z,rotation.W) * OpenTK.Mathematics.Vector3.UnitY;
        return Matrix4x4.CreateLookAt(position, position + new Vector3(forward.X,forward.Y,forward.Z), new Vector3(up.X, up.Y, up.Z));
    }
}