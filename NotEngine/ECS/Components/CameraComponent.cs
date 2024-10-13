using NotEngine.Core;

namespace NotEngine.ECS.Components;

public enum EProjection
{
    Perspective,
    Orthographic,
}

public enum ClearFlags
{
    SolidColor,
    Skybox,
}

public struct CameraComponent() : IEntityComponent
{
    public EProjection Projection { get; set; } = EProjection.Perspective;
    public ClearFlags ClearFlags { get; set; } = ClearFlags.SolidColor;
    public Color Background { get; set; } = Color.CornflowerBlue;
    public float Fov { get; set; } = 45;
    public float Near { get; set; } = 0.3f;
    public float Far { get; set; } = 1000;
    public float Size { get; set; } = 5;
}