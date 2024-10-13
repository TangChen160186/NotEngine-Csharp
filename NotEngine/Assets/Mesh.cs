using System.Numerics;
using NotEngine.Core;
using Veldrid;

namespace NotEngine.Assets; 
public struct MeshVertex
{
    public Vector3 Position { get; set; }
    public Vector3 Normals { get; set; }
    public Vector2 TexCoords { get; set; }
    public Vector3 Tangent { get; set; }
    public Vector3 BiTangent { get; set; }

    public static readonly uint SizeofBytes = 56;

}

public class Mesh : IAsset
{
    public Guid AssetId { get; }
    public EAssetType Type { get; }
    public int RefCount { get; set; }

    private readonly GraphicsDevice _graphicsDevice;
    public bool IsDisposed { get; private set; }
    public MeshVertex[] Vertices { get; set; }
    public uint[] Indices { get; set; }

    // TODO BONE
    public DeviceBuffer VertexBuffer { get; private set; }

    public DeviceBuffer IndexBuffer { get; private set; }

    
    public Mesh( MeshVertex[] vertices, uint[] indices)
    {
        _graphicsDevice = Application.Current.Device;
        Vertices = vertices;
        Indices = indices;
        AssetId = Guid.NewGuid();
        Type = EAssetType.Mesh;
    }
    public Mesh(MeshVertex[] vertices, uint[] indices,Guid guid)
    {
        _graphicsDevice = Application.Current.Device;
        Vertices = vertices;
        Indices = indices;
        AssetId = guid;
        Type = EAssetType.Mesh;
    }
    public void UpLoad()
    {
        BufferDescription vertexBufferDescription = new BufferDescription(MeshVertex.SizeofBytes, BufferUsage.VertexBuffer);
        BufferDescription indexBufferDescription = new BufferDescription(sizeof(uint), BufferUsage.IndexBuffer);

        VertexBuffer = _graphicsDevice.ResourceFactory.CreateBuffer(vertexBufferDescription);
        IndexBuffer = _graphicsDevice.ResourceFactory.CreateBuffer(indexBufferDescription);


    }

    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
        IsDisposed = true;
    }


}