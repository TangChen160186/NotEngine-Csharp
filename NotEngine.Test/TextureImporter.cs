using Assimp;
using MessagePack;
using NotEngine.Assets;
using Mesh = NotEngine.Assets.Mesh;

namespace NotEngine.Test;

public static class TextureImporter
{
    public static Guid Import(string sourcePath,string targetPath, TextureShape shape = TextureShape.Texture2D, bool isCompressed = false, bool isSrgb = false,
        int mipLevels =1, int sampleCount =1)
    {
       var imageResult = ImageHelper.LoadImage(sourcePath);
       Texture texture = new Texture(shape, imageResult.Width, imageResult.Height, 1,isCompressed, isSrgb, mipLevels, sampleCount,
           imageResult.Data,true);
       texture.UnLoadData();
       var result = MessagePackSerializer.Serialize(texture);
       File.WriteAllBytes(Path.Combine(targetPath, $"{texture.AssetId}.asset"),result);
       return texture.AssetId;
    }
}

public static class MeshImporter
{
    public static void Import(string sourcePath, string targetFolderPath)
    {
        using var assimp = new AssimpContext();
        var scene = assimp.ImportFile(sourcePath,PostProcessSteps.Triangulate);

        if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete ||
            scene.RootNode == null) // 加载模型文件失败
        {
            throw new Exception("load exception");
        }

        List<Mesh> meshes = new List<Mesh>();
        ProcessNode(Matrix4x4.Identity, scene, scene.RootNode, meshes);

        for (var i = 0; i < meshes.Count; i++)
        {
            var mesh = meshes[i];
            var bytes = MessagePackSerializer.Serialize(mesh);
            File.WriteAllBytes(Path.Combine(targetFolderPath, $"{mesh.Name}.asset"),bytes);
        }
    }

    private static void ProcessNode(in Matrix4x4 transform, Scene scene, Node rootNode, List<Mesh> meshes)
    {
        var nodeTransform = transform * rootNode.Transform;
        for (int i = 0; i < rootNode.MeshCount; i++)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            Assimp.Mesh mesh = scene.Meshes[rootNode.MeshIndices[i]];

            ProcessMesh(in nodeTransform, mesh, scene, vertices, indices);
            meshes.Add(new Mesh(vertices, indices.ToArray(), mesh.Name));
        }

        for (int i = 0; i < rootNode.ChildCount; i++)
        {
            ProcessNode(nodeTransform, scene, rootNode.Children[i], meshes);
        }
    }

    private static void ProcessMesh(in Matrix4x4 transform, Assimp.Mesh mesh, Scene scene, List<Vertex> outVertices,
        List<uint> outIndices)
    {
        for (var i = 0; i < mesh.VertexCount; i++)
        {
            var position = transform * mesh.Vertices[i];
            var normal = transform * (mesh.HasNormals ? mesh.Normals[i] : new Vector3D(0, 0, 0));
            var texCoords = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0, 0, 0);
            var tangent = mesh.HasTangentBasis ? transform * mesh.Tangents[i] : new Vector3D(0, 0, 0);
            var biTangent = mesh.HasTangentBasis ? transform * mesh.BiTangents[i] : new Vector3D(0, 0, 0);
            var colors = mesh.HasVertexColors(0) ?  mesh.VertexColorChannels[0][i] : new Color4D(0, 0, 0);

            var vertex = new Vertex();
            vertex.Position = vertex.Position with { X = position.X };
            vertex.Position = vertex.Position with { Y = position.Y };
            vertex.Position = vertex.Position with { Z = position.Z };

            vertex.TexCoords = vertex.TexCoords with { X = texCoords.X };
            vertex.TexCoords = vertex.TexCoords with { Y = texCoords.Y };

            vertex.Normals = vertex.Normals with { X = normal.X };
            vertex.Normals = vertex.Normals with { Y = normal.Y };
            vertex.Normals = vertex.Normals with { Z = normal.Z };

            vertex.Tangent = vertex.Tangent with { X = tangent.X };
            vertex.Tangent = vertex.Tangent with { Y = tangent.Y };
            vertex.Tangent = vertex.Tangent with { Z = tangent.Z };

            vertex.BiTangent = vertex.BiTangent with { X = biTangent.X };
            vertex.BiTangent = vertex.BiTangent with { Y = biTangent.Y };
            vertex.BiTangent = vertex.BiTangent with { Z = biTangent.Z };

            vertex.Color = vertex.Color with { X = colors.R };
            vertex.Color = vertex.Color with { Y = colors.G };
            vertex.Color = vertex.Color with { Z = colors.B };
            vertex.Color = vertex.Color with { W = colors.A };
            outVertices.Add(vertex);
        }

        for (var faceId = 0; faceId < mesh.FaceCount; faceId++)
        {
            var face = mesh.Faces[faceId];
            for (var indexId = 0; indexId < face.IndexCount; indexId++)
            {
                outIndices.Add((uint)face.Indices[indexId]);
            }
        }
    }
}