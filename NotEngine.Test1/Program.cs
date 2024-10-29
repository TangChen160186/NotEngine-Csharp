using Assimp;
using NotEngine.Assets;

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

            List<StaticMesh> meshes = new List<StaticMesh>();
            ProcessNode(Matrix4x4.Identity, scene, scene.RootNode, meshes);

            int index = 1;
            foreach (var mesh in meshes)
            {
                MeshSerializer.Serialize(@$"C:\Users\16018\Desktop\AFA\cube{index}.asset",mesh);
            }



        }





    }

    
 
}
