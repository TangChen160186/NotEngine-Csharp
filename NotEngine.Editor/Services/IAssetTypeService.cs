using NotEngine.Editor.Models;

namespace NotEngine.Editor.Services;

public interface IAssetTypeService
{
    AssetType GetAssetType(string extension);
}