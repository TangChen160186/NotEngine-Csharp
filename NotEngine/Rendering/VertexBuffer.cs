namespace NotEngine.Rendering;

public struct VertexBufferLayout
{
    public uint AttributeLocation;
    public bool IsNormalize;
    public int Count;
}

public abstract class VertexBuffer: IDisposable
{
    public abstract bool IsDisposed { get; protected set; }

    public abstract void Dispose();

    public abstract VertexBufferLayout[] Layouts { get; protected set; }


    public int GetStride()
    {
        return Layouts.Sum(e => e.Count) * sizeof(float);
    }
}