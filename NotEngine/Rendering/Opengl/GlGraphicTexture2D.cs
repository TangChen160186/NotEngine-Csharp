using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GlGraphicTexture2D : GraphicTexture2D
{
    private int _id;
    public int Id => _id;

    public override bool IsDisposed { get; protected set; }


    private readonly SizedInternalFormat _internalFormat;

    internal GlGraphicTexture2D(int width,
        int height,
       TextureInternalFormat format,
        bool genMipmap = false)
    {
        Width = width;
        Height = height;
        TextureInternalFormat = format;
        _internalFormat = GLUtils.ToSizeInternalFormat(format);
        (IsSrgb,IsCompressed,CompressFormat) = ConverterUtils.ParseTextureInternalFormat(format);
        GenMipmap = genMipmap;
        GL.CreateTextures(TextureTarget.Texture2d, 1, ref _id);
        MipmapLevels = 1;
        if(genMipmap)
            MipmapLevels = (int)Math.Floor(MathF.Log2(Math.Max(width, height))) + 1;
        GL.TextureStorage2D(_id, MipmapLevels, _internalFormat, width, height);
        CreateTextureHandle();
    }


    private void CreateTextureHandle()
    {
        TextureHandleId = GL.ARB.GetTextureHandleARB(_id);
        GL.ARB.MakeTextureHandleResidentARB(TextureHandleId);
    }



    public override byte[][] UpLoadData(int x, int y, int width, int height, byte[][] data, bool isImport = false)
    {
        int levels = data.Length;

        int currentWidth = width;
        int currentHeight = height;
        for (int i = 0; i < levels; i++)
        {
            if (isImport || !IsCompressed)
            {
                GL.TextureSubImage2D(_id, i, x, y, currentWidth, currentHeight,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte, data[i]);
            }
            else
            {
                GL.CompressedTextureSubImage2D(_id, i, x, y, width, height,
                    (InternalFormat)_internalFormat,
                    data[i].Length, data[i]);
            }
            currentWidth = Math.Max(1, currentWidth / 2);
            currentHeight = Math.Max(1, currentHeight / 2);
        }
        

        if (isImport)
        {
            if (IsCompressed)
            {
                byte[][] compressData = new byte[levels][];
                for (int i = 0; i < levels; i++)
                {
                    int size = 0;
                    GL.GetTextureLevelParameteri(_id, i, (GetTextureParameter)All.TextureCompressedImageSize, ref size);
                    byte[] bytes = new byte[size];
                    GL.GetCompressedTextureImage(_id, i, size, bytes);
                    compressData[i] = bytes;
                }
                return compressData;
            }
        }
        return data;

    }

    public override void Dispose()
    {
        if (!IsDisposed && _id !=0)
        {
            GL.ARB.MakeTextureHandleNonResidentARB(TextureHandleId);
            GL.DeleteTexture(in _id);
            IsDisposed = true;
            _id = 0;
        }
    }
}