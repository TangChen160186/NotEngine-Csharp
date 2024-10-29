using System.Collections.Concurrent;

namespace NotEngine.Graphics;

public static class Graphics
{
    
    public static readonly ConcurrentQueue<IGraphicsObject> ResourcesToDispose = [];
}