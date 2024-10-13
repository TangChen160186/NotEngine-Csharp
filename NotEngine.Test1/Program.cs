using Assimp;
using NotEngine.Assets;
using Mesh = NotEngine.Assets.Mesh;

namespace NotEngine.Test1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Process(@"C:\Users\16018\Desktop\Cube.fbx");

            MeshSerializer.DeSerialize(@$"C:\Users\16018\Desktop\AFA\cube{1}.asset",out var mesh);
        }

        public static void Process(string filePath)
        {

            LoadModel(filePath, PostProcessSteps.GenerateNormals | PostProcessSteps.CalculateTangentSpace);
        }

        public static void LoadModel(string filePath, PostProcessSteps parserFlags)
        {
            using var assimp = new AssimpContext();
            var scene = assimp.ImportFile(filePath, parserFlags);

            if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete ||
                scene.RootNode == null) // 加载模型文件失败
            {
                throw new Exception("load exception");
            }

            List<Mesh> meshes = new List<Mesh>();
            ProcessNode(Matrix4x4.Identity, scene, scene.RootNode, meshes);

            int index = 1;
            foreach (var mesh in meshes)
            {
                MeshSerializer.Serialize(@$"C:\Users\16018\Desktop\AFA\cube{index}.asset",mesh);
            }



        }

        private static void ProcessMaterials(Scene scene, List<string> materials)
        {
            foreach (var mat in scene.Materials)
            {
                if (mat.HasName)
                {
                    materials.Add(mat.Name);
                }
            }
        }

        private static void ProcessNode(in Matrix4x4 transform, Scene scene, Node rootNode, List<Mesh> meshes)
        {
            var nodeTransform = transform * rootNode.Transform;
            for (int i = 0; i < rootNode.MeshCount; i++)
            {
                List<MeshVertex> vertices = new List<MeshVertex>();
                List<uint> indices = new List<uint>();
                Assimp.Mesh mesh = scene.Meshes[rootNode.MeshIndices[i]];
                
                ProcessMesh(in nodeTransform, mesh, scene, vertices, indices);
                meshes.Add(new Mesh(vertices.ToArray(), indices.ToArray()));
            }

            for (int i = 0; i < rootNode.ChildCount; i++)
            {
                ProcessNode(nodeTransform, scene, rootNode.Children[i], meshes);
            }
        }

        private static void ProcessMesh(in Matrix4x4 transform, Assimp.Mesh mesh, Scene scene, List<MeshVertex> outVertices,
            List<uint> outIndices)
        {
            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var position = transform * mesh.Vertices[i];
                var normal = transform * (mesh.HasNormals ? mesh.Normals[i] : new Vector3D(0, 0, 0));
                var texCoords = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0, 0, 0);
                var tangent = mesh.HasTangentBasis ? transform * mesh.Tangents[i] : new Vector3D(0, 0, 0);
                var biTangent = mesh.HasTangentBasis ? transform * mesh.BiTangents[i] : new Vector3D(0, 0, 0);

                var vertex = new MeshVertex();
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

    
 
}
