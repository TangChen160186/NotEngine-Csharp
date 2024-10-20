//using System.Numerics;

//namespace NotEngine.Assets;

//public static class MeshSerializer
//{
//    public static void Serialize(string filePath,StaticMesh staticMesh)
//    {
//        using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));
//        // 写入顶点数量
//        writer.Write(staticMesh.AssetId.ToByteArray());
//        writer.Write(staticMesh.Vertices.Length);
//        foreach (var vertex in staticMesh.Vertices)
//        {
//            writer.Write(vertex.Position.X);
//            writer.Write(vertex.Position.Y);
//            writer.Write(vertex.Position.Z);
//            writer.Write(vertex.Normals.X);
//            writer.Write(vertex.Normals.Y);
//            writer.Write(vertex.Normals.Z);
//            writer.Write(vertex.TexCoords.X);
//            writer.Write(vertex.TexCoords.Y);
//            writer.Write(vertex.Tangent.X);
//            writer.Write(vertex.Tangent.Y);
//            writer.Write(vertex.Tangent.Z);
//            writer.Write(vertex.BiTangent.X);
//            writer.Write(vertex.BiTangent.Y);
//            writer.Write(vertex.BiTangent.Z);
//        }
//        // 写入索引数量
//        writer.Write(staticMesh.Indices.Length);
//        foreach (var index in staticMesh.Indices)
//        {
//            writer.Write(index);
//        }

//        // TODO Bone
//    }
//    public static void DeSerialize(string filePath,out StaticMesh staticMesh)
//    {
//        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));
//        Guid guid = new Guid(reader.ReadBytes(16));
//        // 读取顶点数量
//        int vertexCount = reader.ReadInt32();
//        StaticMeshVertex[] vertices = new StaticMeshVertex[vertexCount];

//        for (int i = 0; i < vertexCount; i++)
//        {
//            float px = reader.ReadSingle();
//            float py = reader.ReadSingle();
//            float pz = reader.ReadSingle();
//            vertices[i].Position = new Vector3(px, py, pz);
//            float nx = reader.ReadSingle();
//            float ny = reader.ReadSingle();
//            float nz = reader.ReadSingle();
//            vertices[i].Normals = new Vector3(nx, ny, nz);
//            float tx = reader.ReadSingle();
//            float ty = reader.ReadSingle();
//            vertices[i].TexCoords = new Vector2(tx, ty);

//            float tax = reader.ReadSingle();
//            float tay = reader.ReadSingle();
//            float taz = reader.ReadSingle();
//            vertices[i].Normals = new Vector3(tax, tay, taz);

//            float bix = reader.ReadSingle();
//            float biy = reader.ReadSingle();
//            float biz = reader.ReadSingle();
//            vertices[i].Normals = new Vector3(bix, biy, biz);
//        }

//        // 读取索引数量
//        int indexCount = reader.ReadInt32();
//        uint[] indices = new uint[indexCount];

//        for (int i = 0; i < indexCount; i++)
//        {
//            indices[i] = reader.ReadUInt32();
//        }
//        staticMesh = new StaticMesh(vertices, indices, guid);
//    }
//}