using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace NotEngine.Test2;

internal static class MipmapHelper
{
    /// <summary>
    /// Computes the number of mipmap levels in a texture.
    /// </summary>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <returns>The number of mipmap levels needed for a texture of the given dimensions.</returns>
    public static int ComputeMipLevels(int width, int height)
    {
        return 1 + (int)Math.Floor(Math.Log(Math.Max(width, height), 2));
    }

    public static int GetDimension(int largestLevelDimension, int mipLevel)
    {
        int ret = largestLevelDimension;
        for (int i = 0; i < mipLevel; i++)
        {
            ret /= 2;
        }

        return Math.Max(1, ret);
    }

    //internal static Image<Rgba32>[] GenerateMipmaps(Image<Rgba32> baseImage)
    //{
    //    int mipLevelCount = MipmapHelper.ComputeMipLevels(baseImage.Width, baseImage.Height);
    //    Image<Rgba32>[] mipLevels = new Image<Rgba32>[mipLevelCount];
    //    mipLevels[0] = baseImage;
    //    int i = 1;

    //    int currentWidth = baseImage.Width;
    //    int currentHeight = baseImage.Height;
    //    while (currentWidth != 1 || currentHeight != 1)
    //    {
    //        int newWidth = Math.Max(1, currentWidth / 2);
    //        int newHeight = Math.Max(1, currentHeight / 2);
    //        Image<Rgba32> newImage = baseImage.Clone(context => context.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));
    //        Debug.Assert(i < mipLevelCount);
    //        mipLevels[i] = newImage;

    //        i++;
    //        currentWidth = newWidth;
    //        currentHeight = newHeight;
    //    }

    //    Debug.Assert(i == mipLevelCount);

    //    return mipLevels;
    //}


    internal static byte[][] GenerateMipmaps(Image<Rgba32> baseImage)
    {
        int mipLevelCount = MipmapHelper.ComputeMipLevels(baseImage.Width, baseImage.Height);
        byte[][] mipLevels = new byte[mipLevelCount][];

        int currentWidth = baseImage.Width;
        int currentHeight = baseImage.Height;

        for (int i = 0; i < mipLevelCount; i++)
        {
            // Clone and resize the image for the current mip level
            using Image<Rgba32> mipImage = i == 0 ? baseImage.Clone() : baseImage.Clone(context => context.Resize(currentWidth, currentHeight, KnownResamplers.Lanczos3));

            // Allocate byte array for raw pixel data and copy pixel data into it
            byte[] pixelData = new byte[currentWidth * currentHeight * 4]; // 4 bytes per pixel for RGBA
            mipImage.CopyPixelDataTo(pixelData);
            mipLevels[i] = pixelData;

            // Update dimensions for the next mip level
            currentWidth = Math.Max(1, currentWidth / 2);
            currentHeight = Math.Max(1, currentHeight / 2);
        }

        return mipLevels;
    }
}