using System.ComponentModel.Composition;
using NotEngine.Editor.Models;

namespace NotEngine.Editor.Services;

[Export(typeof(IAssetTypeService))]
public class FileTypeService : IAssetTypeService
{
    public AssetType GetAssetType(string extension)
    {
        return extension switch
        {
            "" => AssetType.Folder,
            ".texture" => AssetType.Texture,
            _ => AssetType.UnKnow
        };
    }
}