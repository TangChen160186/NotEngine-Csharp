using StbImageSharp;

namespace NotEngine.Test;

public static class ImageHelper
{
    public static ImageResult LoadImage(string imgPath)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        return ImageResult.FromMemory(File.ReadAllBytes(imgPath), ColorComponents.RedGreenBlueAlpha);
        
    }
}