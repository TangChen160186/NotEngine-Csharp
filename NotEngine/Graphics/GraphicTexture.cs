using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public abstract class GraphicTexture : IDisposable
{
    protected int _id;

    public int Id => _id;

    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public int Depth { get; protected set; }
    public bool IsCompressed { get; protected set; }
    public bool IsSrgb { get; protected set; }
    public int SampleCount { get; protected set; }
    public int MipLevels { get; protected set; }
    public ulong Size { get; protected set; }

    private ulong? _textureHandleId;

    public ulong TextureHandleId => _textureHandleId ?? CreateTextureHandle();
    public SizedInternalFormat InternalFormat { get; protected set; }

    public bool IsDisposed { get; protected set; }


    protected GraphicTexture(int width, int height, int depth, SizedInternalFormat internalFormat, bool isCompressed, bool isSrgb,
        int mipLevels, int sampleCount)
    {
        Width = width;
        Height = height;
        Depth = depth;
        InternalFormat = internalFormat;
        IsCompressed = isCompressed;    
        IsSrgb = isSrgb;
        SampleCount = sampleCount;
        MipLevels = mipLevels;
    }

    protected ulong CreateTextureHandle()
    {
        _textureHandleId = GL.ARB.GetTextureHandleARB(_id);
        GL.ARB.MakeTextureHandleResidentARB(_textureHandleId.Value);
        return _textureHandleId.Value;
    }

   

    public abstract void UpLoad(int x, int y, int z, int width, int height, int depth,
        byte[] data,bool isCompressed);

    public void UpLoad(int x, int y, int width, int height,
        byte[] data, bool isCompressed)
    {
        UpLoad(x, y, 0, width, height, 1, data,isCompressed);
        Size = (ulong)data.Length;
    }


    public abstract byte[] UnLoad();

    public void Dispose()
    {
        if (!IsDisposed)
        {
            if (_textureHandleId != null)
                GL.ARB.MakeTextureHandleNonResidentARB(TextureHandleId);
            GL.DeleteTexture(in _id);
            IsDisposed = true;
        }
    }
}

public sealed class GraphicTexture2D : GraphicTexture
{
    public GraphicTexture2D(int width,
        int height,
        bool isCompressed,
        SizedInternalFormat format, 
        bool isSrgb = false,
        int mipLevels = 1
    ) :
        base(width, height, 1,format, isCompressed, isSrgb, mipLevels,1)
    {

        GL.CreateTextures(TextureTarget.Texture2d, 1, ref _id);
        GL.TextureStorage2D(_id, mipLevels, format, width, height);
    }


    public override void UpLoad(int x,
        int y, int z,
        int width,
        int height,
        int depth,
        byte[] data,bool isCompressed)
    {

        if (!isCompressed)
            GL.TextureSubImage2D(_id, 0, x, y, width, height,
                PixelFormat.Rgba,
                PixelType.UnsignedByte, data);
        else
        {
            GL.CompressedTextureSubImage2D(_id, 0, x, y, width, height,
                (InternalFormat)InternalFormat,
                data.Length, data);
        }

        if (MipLevels > 1)
        {
            GL.GenerateTextureMipmap(_id);
        }
    }


    public override byte[] UnLoad()
    {
        int isCompressedTexture = 0;
        GL.GetTextureLevelParameteri(_id, 0, (GetTextureParameter)All.TextureCompressed, ref isCompressedTexture);

        int size = 0;
        if (isCompressedTexture == (int)All.True)
            GL.GetTextureLevelParameteri(_id, 0, (GetTextureParameter)All.TextureCompressedImageSize, ref size);
        else
            size = Width * Height * 4;
        byte[] bytes = new byte[size];
        if (isCompressedTexture == (int)All.True)
            GL.GetCompressedTextureImage(_id, 0, size, bytes);
        else
            GL.GetTextureImage(_id, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, size, bytes);
        GL.Finish();
        return bytes;
    }
}