
using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal static class GLUtils
{
    public static SizedInternalFormat ToSizeInternalFormat(TextureInternalFormat format)
    {
        switch (format)
        {
            case TextureInternalFormat.RG16_UNORM:
                return SizedInternalFormat.Rg16;

            case TextureInternalFormat.R16_UNORM:
                return SizedInternalFormat.R16;

            case TextureInternalFormat.RGB8_UNORM:
                return SizedInternalFormat.Rgb8;

            case TextureInternalFormat.RGBA8_UNORM:
                return SizedInternalFormat.Rgba8;

            case TextureInternalFormat.BC1_RGBA_UNORM:
                return SizedInternalFormat.CompressedRgbaS3tcDxt1Ext;

            case TextureInternalFormat.BC2_RGBA_UNORM:
                return SizedInternalFormat.CompressedRgbaS3tcDxt3Ext;

            case TextureInternalFormat.BC3_RGBA_UNORM:
                return SizedInternalFormat.CompressedRgbaS3tcDxt5Ext;

            case TextureInternalFormat.BC6H_RGB_UNORM:
                return SizedInternalFormat.CompressedRgbBptcUnsignedFloat;

            case TextureInternalFormat.BC7_RGBA_UNORM:
                return SizedInternalFormat.CompressedSrgbAlphaBptcUnorm;


            case TextureInternalFormat.BC1_SRGBA_UNORM:
                return SizedInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext;

            case TextureInternalFormat.BC2_SRGBA_UNORM:
                return SizedInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext;

            case TextureInternalFormat.BC3_SRGBA_UNORM:
                return SizedInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
            case TextureInternalFormat.BC7_SRGBA_UNORM:
                return SizedInternalFormat.CompressedRgbaBptcUnorm;
            case TextureInternalFormat.Depth32fStencil8:
                return SizedInternalFormat.Depth32fStencil8;
            case TextureInternalFormat.SRGB8_UNORM:
                return SizedInternalFormat.Srgb8Alpha8;
            case TextureInternalFormat.SRGBA8_UNORM:
                return SizedInternalFormat.Srgb8;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}