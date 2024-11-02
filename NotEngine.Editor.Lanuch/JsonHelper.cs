using System.IO;
using System.Text.Json;

namespace NotEngine.Editor.Lanuch
{
    public static class JsonHelper
    {
        // 序列化对象为 JSON 字符串并保存到文件
        public static void SerializeToFile<T>(T obj, string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(obj, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialization Error: " + ex.Message);
            }
        }

        // 从文件反序列化为对象
        public static T DeserializeFromFile<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<T>(jsonString);
                }
                else
                {
                    throw new FileNotFoundException("File not found: " + filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialization Error: " + ex.Message);
                return default;
            }
        }

        // 将对象序列化为 JSON 字符串
        public static string SerializeToString<T>(T obj)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                return JsonSerializer.Serialize(obj, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialization Error: " + ex.Message);
                return null;
            }
        }

        // 从 JSON 字符串反序列化为对象
        public static T DeserializeFromString<T>(string jsonString)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialization Error: " + ex.Message);
                return default;
            }
        }
    }
}