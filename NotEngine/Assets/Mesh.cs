using System.Numerics;
using MessagePack;
using NotEngine.Graphics;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true)]
public struct Vertex
{
    public Vector3 Position { get; set; }
    public Vector2 TexCoords { get; set; }
    public Vector3 Normals { get; set; }
    public Vector3 Tangent { get; set; }
    public Vector3 BiTangent { get; set; }
    public Vector4 Color { get; set; }

    public static VertexBufferLayout[] VertexLayouts = [
        new(){AttributeLocation = 0,Count = 3},
        new(){AttributeLocation = 1,Count = 2},
        new(){AttributeLocation = 2,Count = 3},
        new(){AttributeLocation = 3,Count = 3},
        new(){AttributeLocation = 4,Count = 3},
        new(){AttributeLocation = 5,Count = 4},
    ];
}
[MessagePackObject(keyAsPropertyName:true)]
public class Mesh: IAsset
{
    [IgnoreMember]
    private VertexArray _vao;
    [IgnoreMember]
    private VertexBuffer _vbo;
    [IgnoreMember]
    private IndexBuffer _ibo;
    
    public Guid AssetId { get; }
    public string Name { get;  }
    [IgnoreMember]
    public AssetType Type => AssetType.Mesh;
    [IgnoreMember]
    public int VertexCount { get; }
    [IgnoreMember]
    public int IndexCount { get; }

    public List<Vertex> Vertices { get; }
    public uint[] Indices { get; }


    [SerializationConstructor]
    public Mesh(Guid assetId,List<Vertex> vertices, uint[] indices,string name)
    {
        Name = name;
        AssetId = assetId;
        VertexCount = vertices.Count;
        IndexCount = indices.Length;
        SetupMesh(vertices, indices);
        Vertices = vertices;
        Indices = indices;
    }
    public Mesh(List<Vertex> vertices, uint[] indices,string name)
    {
        Name = name;
        Vertices = vertices;
        Indices = indices;
        AssetId = Guid.NewGuid();
        VertexCount = vertices.Count;
        IndexCount = indices.Length;
        SetupMesh(vertices,indices);
    }

    private void SetupMesh(List<Vertex> vertices, uint[] indices)
    {
     
        List<float> vertexData = new List<float>(vertices.Count);
        foreach (var vertex in vertices)
        {
            vertexData.Add(vertex.Position.X);
            vertexData.Add(vertex.Position.Y);
            vertexData.Add(vertex.Position.Z);
            vertexData.Add(vertex.TexCoords.X);
            vertexData.Add(vertex.TexCoords.Y);
            vertexData.Add(vertex.Normals.X);
            vertexData.Add(vertex.Normals.Y);
            vertexData.Add(vertex.Normals.Z);
            vertexData.Add(vertex.Tangent.X);
            vertexData.Add(vertex.Tangent.Y);
            vertexData.Add(vertex.Tangent.Z);
            vertexData.Add(vertex.BiTangent.X);
            vertexData.Add(vertex.BiTangent.Y);
            vertexData.Add(vertex.BiTangent.Z);
            vertexData.Add(vertex.Color.X);
            vertexData.Add(vertex.Color.Y);
            vertexData.Add(vertex.Color.Z);
            vertexData.Add(vertex.Color.W);
        }

        _vao = new VertexArray();
        _vbo = new VertexBuffer(vertexData.ToArray(), Vertex.VertexLayouts);
        _ibo = new IndexBuffer(indices);
        _vao.BindIndexBuffer(_ibo);
        _vao.BindVertexBuffer(_vbo);
    }
    public void Bind()
    {
        _vao.Bind();
    }

    public void UnBind()
    {
        _vao.Unbind();
    }
    public void Dispose()
    {
        _vao.Dispose();
        _vbo.Dispose();
        _ibo.Dispose();
    }
}
