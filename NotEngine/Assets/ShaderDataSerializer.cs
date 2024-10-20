//namespace NotEngine.Assets;

//public static class ShaderDataSerializer
//{
//    public static void Serialize(string filePath, ShaderData shaderData)
//    {
//        using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));
//        writer.Write(shaderData.AssetId.ToByteArray());
//        writer.Write(shaderData.VertexSourceBytes.Length);
//        writer.Write(shaderData.VertexSourceBytes);
//        writer.Write(shaderData.FragmentSourceBytes.Length);
//        writer.Write(shaderData.FragmentSourceBytes);
//    }

//    public static void DeSerialize(string filePath, out ShaderData shaderData)
//    {
//        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));
//        Guid guid = new Guid(reader.ReadBytes(16));
//        int vertexSourceBytesLength = reader.ReadInt32();
//        byte[] vertexSourceBytes = reader.ReadBytes(vertexSourceBytesLength);

//        int fragmentSourceBytesLength = reader.ReadInt32();
//        byte[] fragmentSourceBytes = reader.ReadBytes(fragmentSourceBytesLength);
//        shaderData = new ShaderData(vertexSourceBytes, fragmentSourceBytes);
//    }
//}