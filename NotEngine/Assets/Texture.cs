using MessagePack;
using NotEngine.Rendering;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true)]
public partial class Texture2D : IAsset
{
    public Guid AssetId { get; }
    public int Width => GraphicTexture.Width;
    public int Height => GraphicTexture.Height;
    public bool IsCompressed => GraphicTexture.IsCompressed;
    public bool IsSrgb => GraphicTexture.IsSrgb;
    [IgnoreMember] public int MipLevels => GraphicTexture.MipmapLevels;
    public ulong Size => (ulong)Data.Sum(byteArray => byteArray.Length);
    public bool GenMipmap => GraphicTexture.GenMipmap;
    public CompressFormat ComPressFormat => GraphicTexture.CompressFormat;
    public TextureInternalFormat Format => GraphicTexture.TextureInternalFormat;
    public byte[][] Data { get; }

    [IgnoreMember] public ulong TextureHandleId => GraphicTexture.TextureHandleId;
    [IgnoreMember] public GraphicTexture2D GraphicTexture { get; set; }

    [SerializationConstructor]
    private Texture2D(Guid assetId, int width, int height, bool isCompressed, CompressFormat compressFormat, bool isSrgb,bool genMipmap, byte[][] data)
    {
        AssetId = assetId;
        GraphicTexture =
            Graphics.Device.CreateGraphicTexture2D(width, height,
                ToTextureInternalFormat(compressFormat, isCompressed, isSrgb), genMipmap);
        GraphicTexture.UpLoadData(0, 0, width, height, data);
        Data = data;
    }

    public Texture2D(int width, int height, bool isCompressed, CompressFormat format, bool isSrgb, bool genMipmap, byte[][] data)
    {
        AssetId = Guid.NewGuid();
        GraphicTexture =
            Graphics.Device.CreateGraphicTexture2D(width, height,
                ToTextureInternalFormat(format, isCompressed, isSrgb), genMipmap);
        Data = GraphicTexture.UpLoadData(0, 0, width, height, data, true);
    }


    private static TextureInternalFormat ToTextureInternalFormat(CompressFormat compressFormat, bool isCompressed,
        bool isSrgb)
    {
        if (!isCompressed)
            return isSrgb ? TextureInternalFormat.SRGBA8_UNORM : TextureInternalFormat.RGBA8_UNORM;

        switch (compressFormat)
        {
            case CompressFormat.BC1:
                return isSrgb
                    ? TextureInternalFormat.BC1_SRGBA_UNORM
                    : TextureInternalFormat.BC1_RGBA_UNORM;
            case CompressFormat.BC2:
                return isSrgb
                    ? TextureInternalFormat.BC2_SRGBA_UNORM
                    : TextureInternalFormat.BC2_RGBA_UNORM;
            case CompressFormat.BC3:
                return isSrgb
                    ? TextureInternalFormat.BC3_SRGBA_UNORM
                    : TextureInternalFormat.BC3_RGBA_UNORM;
            case CompressFormat.BC6H:
                return TextureInternalFormat.BC6H_RGB_UNORM;
            case CompressFormat.BC7:
                return isSrgb
                    ? TextureInternalFormat.BC7_SRGBA_UNORM
                    : TextureInternalFormat.BC7_RGBA_UNORM;
            default:
                throw new ArgumentOutOfRangeException(nameof(compressFormat), compressFormat, null);
        }
    }

    public void Dispose()
    {
        GraphicTexture.Dispose();
    }
}