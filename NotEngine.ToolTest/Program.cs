using System.Text;
using MessagePack;

namespace NotEngine.ToolTest
{

    public abstract class IInterface
    {
        [Key("D")] public string D { get; set; } = "faf";
    }
    [MessagePackObject]
    public class Test: IInterface
    {
        [Key("A")]
        public int A { get; set; }

        [Key("B")] public String B { get; }

        [SerializationConstructor]
        public Test()
        {
            B = "B";
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Test t = new Test();
            t.A = 1;
       
            var s = MessagePackSerializer.Serialize(t,MessagePackSerializerOptions.Standard);
            var json = MessagePackSerializer.SerializeToJson(t);
            var m = MessagePackSerializer.Deserialize<Test>(s);
            Console.WriteLine(s);
        }
    }
}
