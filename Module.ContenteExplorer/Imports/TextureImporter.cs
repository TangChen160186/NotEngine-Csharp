using MessagePack;
using NotEngine.Assets;
using StbImageSharp;
using System.IO;

namespace Module.ContentExplorer.Imports;

public static class TextureImporter
{
    public static Guid Import(string sourcePath, string targetPath, 
        TextureShape shape = TextureShape.Texture2D, 
        bool isCompressed = false,
        bool isSrgb = false,
        int mipLevels = 1, 
        int sampleCount = 1)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        var imageResult = ImageResult.FromMemory(File.ReadAllBytes(sourcePath), ColorComponents.RedGreenBlueAlpha);
        Texture texture = new Texture(shape, imageResult.Width, imageResult.Height, 1, isCompressed, isSrgb, mipLevels, sampleCount,
            imageResult.Data, true);
        texture.UnLoadData();
        var result = MessagePackSerializer.Serialize(texture);
        File.WriteAllBytes(targetPath, result);
        return texture.AssetId;

    }
}