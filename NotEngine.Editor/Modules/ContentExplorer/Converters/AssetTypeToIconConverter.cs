using System.Globalization;
using System.Windows.Data;
using NotEngine.Editor.Models;

namespace NotEngine.Editor.Modules.ContentExplorer.Converters;

internal class AssetTypeToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.UnKnow:
                    break;
                case AssetType.Folder:
                    break;
                case AssetType.Texture:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        throw new ArgumentOutOfRangeException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}