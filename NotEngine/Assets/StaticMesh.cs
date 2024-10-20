using System.Numerics;
namespace NotEngine.Assets; 
public struct StaticMeshVertex
{   
    public Vector3 Position { get; set; }
    public Vector3 Normals { get; set; }
    public Vector2 TexCoords { get; set; }
    public Vector3 Tangent { get; set; }
    public Vector3 BiTangent { get; set; }

    public static readonly uint SizeofBytes = 56;
}
public interface IMesh : IDisposable
{
    void Bind();
    void UnBind();
    int VertexCount { get; }
    int IndexCount { get; }
}


public class StaticMesh : AssetBase,IMesh
{
    public override string Name { get; internal set; }
    public void Bind()
    {
        throw new NotImplementedException();
    }

    public void UnBind()
    {
        throw new NotImplementedException();
    }

    public int VertexCount { get; }
    public int IndexCount { get; }




    public override bool IsDisposed { get; protected set; }
    public override EAssetType Type { get; internal set; }


    public override void Dispose()
    {
        IsDisposed = true;
    }


}