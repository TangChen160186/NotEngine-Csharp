using SpirvReflector;

namespace NotEngine.ToolTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize a logger and SpirvReflection instance. A single instance can safely be used to reflect multiple SPIR-V bytecode files and is thread-safe.
            IReflectionLogger log = new SpirvConsoleLogger();
            SpirvReflectionFlags flags = SpirvReflectionFlags.None;
            
            SpirvReflection reflection = new SpirvReflection(log);

            // Load each .spirv bytecode file in the current directory and run SpirvReflection.Reflect() on it.
         
            byte[] byteCode = null;
            using (FileStream stream = new FileStream("G:\\veldrid\\src\\NeoDemo\\Assets\\Shaders\\ShadowMain.vert.spv", FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                    byteCode = reader.ReadBytes((int)stream.Length);
            }
            
            SpirvReflectionResult result = reflection.Reflect(byteCode, flags);
            Console.WriteLine("--------------------------------------------------");
            

            log.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
