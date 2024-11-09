namespace NotEngine.Rendering;

public enum CompressFormat
{
    BC1,
    BC2,
    BC3,
    BC6H,
    BC7,
}



public abstract class GraphicTexture2D : IDisposable
{
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public bool IsCompressed { get; protected set; }
    public CompressFormat CompressFormat { get; protected set; }
    public TextureInternalFormat TextureInternalFormat { get; protected set; }
    public bool IsSrgb { get; protected set; }
    public bool GenMipmap { get; protected set; }
    public int MipmapLevels { get; protected set; }

    public abstract bool IsDisposed { get; protected set; }
    public  ulong TextureHandleId { get; protected set; }
    public abstract void Dispose();

    public abstract byte[][] UpLoadData(int x, int y, int width, int height, byte[][] data,
        bool isImport = false);
}