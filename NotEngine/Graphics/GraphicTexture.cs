using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace NotEngine.Graphics;

public enum TextureType
{
    Texture1D,
    Texture2D,
    Texture3D,
    TextureCube,
}

public abstract class GraphicTexture : IDisposable
{
    protected int _id;

    public int Id=>_id;
   
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public int Depth { get; protected set; }
    public bool IsCompressed { get; protected set; }
    public bool IsSrgb { get; protected set; }
    public int SampleCount { get; protected set; }
    public int MipLevels { get; protected set; }
    public ulong Size { get; protected set; }
    public bool IsDisposed { get; protected set; }
    public ulong TextureHandleId { get; protected set; }

    public string CurrentInternalFormat => GetCurrentInternalFormat().ToString();


    protected GraphicTexture(int width, int height, int depth, bool isCompressed, bool isSrgb = false,
        int mipLevels = 1, int sampleCount = 1)
    {
        Width = width;
        Height = height;
        Depth = depth;
        IsCompressed = isCompressed;
        IsSrgb = isSrgb;
        SampleCount = sampleCount;
        MipLevels = mipLevels;
    }

    protected void CreateTextureHandle()
    {
        TextureHandleId = GL.ARB.GetTextureHandleARB(_id);
        GL.ARB.MakeTextureHandleResidentARB(TextureHandleId);
    }

    protected SizedInternalFormat GetCurrentInternalFormat()
    {
        SizedInternalFormat internalFormat = SizedInternalFormat.Rgba8;
        if (!IsCompressed)
        {
            if (IsSrgb)
            {
                internalFormat = SizedInternalFormat.Srgb8Alpha8;
            }
        }
        else
        {
            if (IsSrgb)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    internalFormat = SizedInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    internalFormat = SizedInternalFormat.CompressedRgbaS3tcDxt5Ext;
            }
        }

        return internalFormat;
    }

    public abstract void UpLoad(int x, int y, int z, int width, int height, int depth,
        byte[] data);

    public void UpLoad(int x, int y, int width, int height,
        byte[] data, bool isCompressed)
    {
        UpLoad(x, y, 0, width, height, 1, data);
    }


    public abstract byte[] UnLoad();

    ~GraphicTexture() => ReleaseUnmanagedResources();

    private void ReleaseUnmanagedResources()
    {
        GL.ARB.MakeTextureHandleNonResidentARB(TextureHandleId);
        GL.DeleteTexture(in _id);
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }
}

public sealed class GraphicTexture2D : GraphicTexture
{
    public GraphicTexture2D(int width,
        int height,
        bool isCompressed,
        bool isSrgb = false,
        int mipLevels = 1, 
        SizedInternalFormat? format = null) :
        base(width, height, 1, isCompressed, isSrgb, mipLevels)
    {
        MipLevels = mipLevels;
        GL.CreateTextures(TextureTarget.Texture2d, 1, ref _id);
        GL.TextureStorage2D(_id, mipLevels, format ?? GetCurrentInternalFormat(), width, height);
        CreateTextureHandle();
    }


    public override void UpLoad(int x,
        int y, int z,
        int width,
        int height,
        int depth,
        byte[] data)
    {
        if (!IsCompressed)
            GL.TextureSubImage2D(_id, 0, x, y, width, height,
                PixelFormat.Rgba,
                PixelType.UnsignedByte, data);
        else
        {
            GL.CompressedTextureSubImage2D(_id, 0, x, y, width, height,
                (InternalFormat)GetCurrentInternalFormat(),
                sizeof(byte) * data.Length, data);
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
        return bytes;
    }
}

