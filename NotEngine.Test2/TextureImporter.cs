using Assimp;
using MessagePack;
using NotEngine.Assets;
using NotEngine.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Mesh = NotEngine.Assets.Mesh;

namespace NotEngine.Test2;

public static class TextureImporter
{
    public static void Import2D(string sourcePath, string targetPath, bool isCompressed, CompressFormat format, bool isSrgb,
        bool genMipMap)
    {
        // 加载图像
        using Image<Rgba32> baseImage = Image.Load<Rgba32>(sourcePath);
        // 清理现有的 GL 资源或在同一上下文中创建新对象
        Texture2D texture = null;
        try
        {
            var mipmaps = MipmapHelper.GenerateMipmaps(baseImage);

            texture = new Texture2D(
                baseImage.Width,
                baseImage.Height,
                isCompressed,
                format,
                isSrgb,
                genMipMap,
                genMipMap ? mipmaps : new byte[][] { mipmaps[0] }
            );
            var bytes = MessagePackSerializer.Serialize(texture);
            File.WriteAllBytes(Path.Combine(targetPath, @$"{format}.2d"), bytes);

        }
        finally
        {
            texture?.Dispose();
        }
    }
}

public static class MeshImporter
{
    public static void Import(string sourcePath, string targetFolderPath)
    {
        using var assimp = new AssimpContext();
        var scene = assimp.ImportFile(sourcePath, PostProcessSteps.Triangulate);

        if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete ||
            scene.RootNode == null) // 加载模型文件失败
        {
            throw new Exception("load exception");
        }

        List<Mesh> meshes = new List<Mesh>();
        ProcessNode(scene, scene.RootNode, meshes);

        for (var i = 0; i < meshes.Count; i++)
        {
            var mesh = meshes[i];
            var bytes = MessagePackSerializer.Serialize(mesh);
            File.WriteAllBytes(Path.Combine(targetFolderPath, $"{mesh.Name}.asset"), bytes);
        }
    }

    private static void ProcessNode(Scene scene, Node rootNode, List<Mesh> meshes)
    {
        for (int i = 0; i < rootNode.MeshCount; i++)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            Assimp.Mesh mesh = scene.Meshes[rootNode.MeshIndices[i]];

            ProcessMesh(mesh, scene, vertices, indices);
            meshes.Add(new Mesh(vertices, indices.ToArray(), mesh.Name));
        }

        for (int i = 0; i < rootNode.ChildCount; i++)
        {
            ProcessNode(scene, rootNode.Children[i], meshes);
        }
    }

    private static void ProcessMesh(Assimp.Mesh mesh, Scene scene, List<Vertex> outVertices,
        List<uint> outIndices)
    {
        for (var i = 0; i < mesh.VertexCount; i++)
        {
            var position = mesh.Vertices[i];
            var normal = (mesh.HasNormals ? mesh.Normals[i] : new Vector3D(0, 0, 0));
            var texCoords = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0, 0, 0);
            var tangent = mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D(0, 0, 0);
            var biTangent = mesh.HasTangentBasis ? mesh.BiTangents[i] : new Vector3D(0, 0, 0);
            var colors = mesh.HasVertexColors(0) ? mesh.VertexColorChannels[0][i] : new Color4D(0, 0, 0);

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