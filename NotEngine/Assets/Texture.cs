using MessagePack;
using NotEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NotEngine.Assets;
public enum TextureShape: byte
{
    Texture2D,
}

[MessagePackObject(keyAsPropertyName:true)]
public partial class Texture: IAsset
{
    public Guid AssetId { get;}
    public TextureShape Shape { get; private set; }
    public int Width => GraphicTexture.Width;
    public int Height => GraphicTexture.Height;
    public int Depth =>GraphicTexture.Depth;
    public bool IsCompressed => GraphicTexture.IsCompressed;
    public bool IsSrgb => GraphicTexture.IsSrgb;
    public int SampleCount => GraphicTexture.SampleCount;
    public int MipLevels => GraphicTexture.MipLevels;
    public ulong Size => GraphicTexture.Size;
    [IgnoreMember]
    public SizedInternalFormat Format => GraphicTexture.InternalFormat;
    public byte[] Data { get; private set; }

    [IgnoreMember]
    public ulong TextureHandleId => GraphicTexture.TextureHandleId;
    [IgnoreMember]
    public GraphicTexture GraphicTexture { get; set; }

    [SerializationConstructor]
    private Texture(Guid assetId, TextureShape shape, int width, int height, int depth, bool isCompressed, bool isSrgb,
        int mipLevels, int sampleCount, byte[] data)
    {
        Shape = shape;
        AssetId = assetId;
        CreateTexture(shape, width, height, depth, isCompressed, isSrgb, mipLevels, sampleCount, data, false);
    }

    public Texture(TextureShape shape, int width, int height, int depth, bool isCompressed, bool isSrgb,
        int mipLevels, int sampleCount, byte[] data,bool isImport = false)
    {
        Shape = shape;
        AssetId = Guid.NewGuid();
        CreateTexture(shape, width, height, depth, isCompressed, isSrgb, mipLevels, sampleCount, data,isImport);
    }

    public byte[] UnLoadData()
    {
        Data = GraphicTexture.UnLoad();
        return Data;
    }

    
    public void CreateTexture(TextureShape shape, int width, int height, int depth, bool isCompressed, bool isSrgb,
        int mipLevels, int sampleCount, byte[] data, bool isImport)
    {
    
        switch (shape)
        {
            case TextureShape.Texture2D:
                GraphicTexture = new GraphicTexture2D(width, height, isCompressed, GetCurrentInternalFormat(isCompressed, isSrgb), isSrgb, mipLevels);
                GraphicTexture.UpLoad(0, 0, width, height, data, !isImport && isCompressed);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
        }
    }


    private SizedInternalFormat GetCurrentInternalFormat(bool isCompressed, bool isSrgb)
    {
        SizedInternalFormat internalFormat = SizedInternalFormat.Rgba8;
        if (!isCompressed)
        {
            if (isSrgb)
            {
                internalFormat = SizedInternalFormat.Srgb8Alpha8;
            }
        }
        else
        {
            if (isSrgb)
            {
                internalFormat = SizedInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
            }
            else
            {
     
                internalFormat = SizedInternalFormat.CompressedRgbaS3tcDxt5Ext;
            }
        }

        return internalFormat;
    }

    public void Dispose()
    {
        GraphicTexture.Dispose();
    }
}

