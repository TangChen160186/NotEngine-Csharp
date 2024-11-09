using System.Collections.Concurrent;
using MessagePack;

namespace NotEngine.Assets;

[MessagePackObject(keyAsPropertyName: true,SuppressSourceGeneration = false)]
public partial class AssetMap
{
    [IgnoreMember] public static  string ProjectPath { get; set; }
    [IgnoreMember] public static string AssetPath { get; set; }
    private static  Lazy<AssetMap> _instance;
    [IgnoreMember] public static AssetMap Instance => _instance.Value;

    public ConcurrentDictionary<Guid, MetaData> MetaDatas { get; private set; } = [];
    public static void Init(string projectPath,string assetPath)
    {
        ProjectPath = projectPath;
        AssetPath = assetPath;
        var s = File.ReadAllBytes(Path.Combine(projectPath, "AssetMap.json"));
        if (s.Length > 0)
        {
            _instance = new(MessagePackSerializer.Deserialize<AssetMap>(
                s));
        }
        else
        {
            _instance = new Lazy<AssetMap>();
        }
    }

    public MetaData? GetMetaData(Guid id)
    {
        return MetaDatas.GetValueOrDefault(id);
    }

    public string? GetAssetPath(Guid id)
    {
        var metaData = GetMetaData(id);
        if(metaData!=null)
            return Path.Combine(ProjectPath, metaData.AssetPath);
        return null;
    }
    public Guid? GetIdByAssetPath(string path)
    {
        foreach (var (key, value) in MetaDatas)
        {
            if (value.AssetPath == path)
            {
                return key;
            }
        }
        return null;
    }

    public void AddMetaData(Guid id,MetaData data)
    {
        MetaDatas[id] = data;
    }

    public void Save()
    {
        var bytes =MessagePackSerializer.Serialize<AssetMap>(this);
        File.WriteAllBytes((Path.Combine(ProjectPath, "AssetMap.json")), bytes);
    }
}