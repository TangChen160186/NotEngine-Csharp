namespace NotEngine.Rendering;

internal static class ConverterUtils
{
    public static (bool, bool, CompressFormat) ParseTextureInternalFormat(TextureInternalFormat format)
    {
        switch (format)
        {
            case TextureInternalFormat.RG16_UNORM:
                return (false, false, default);
            case TextureInternalFormat.R16_UNORM:
                return (false, false, default);
            case TextureInternalFormat.RGB8_UNORM:
                return (false, false, default);
            case TextureInternalFormat.RGBA8_UNORM:
                return (false, false, default);
            case TextureInternalFormat.BC1_RGBA_UNORM:
                return (false, true, CompressFormat.BC1);
            case TextureInternalFormat.BC2_RGBA_UNORM:
                return (false, true, CompressFormat.BC2);
            case TextureInternalFormat.BC3_RGBA_UNORM:
                return (false, true, CompressFormat.BC3);
            case TextureInternalFormat.BC6H_RGB_UNORM:
                return (false, true, CompressFormat.BC6H);
            case TextureInternalFormat.BC7_RGBA_UNORM:
                return (false, true, CompressFormat.BC7);
            case TextureInternalFormat.BC1_SRGBA_UNORM:
                return (true, true, CompressFormat.BC1);
            case TextureInternalFormat.BC2_SRGBA_UNORM:
                return (true, true, CompressFormat.BC2);
            case TextureInternalFormat.BC3_SRGBA_UNORM:
                return (true, true, CompressFormat.BC3);
            case TextureInternalFormat.BC7_SRGBA_UNORM:
                return (true, true, CompressFormat.BC7);
            case TextureInternalFormat.Depth32fStencil8:
                return (false, false, default);
            case TextureInternalFormat.SRGB8_UNORM:
                return (true, false, default);
            case TextureInternalFormat.SRGBA8_UNORM:
                return (true, false, default);
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}