using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using NotEngine.Editor.Modules.ContentExplorer.Core.FileType;
using NotEngine.Editor.Modules.ContentExplorer.Servies;

namespace NotEngine.Editor.Modules.ContentExplorer.Converters;

internal class FileTypeToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IFileType fileItemType)
            return new Path
            {
                Data = Geometry.Parse(IoC.Get<IFileTypeService>().GetFileTypePath(fileItemType)), Fill =
                    Application.Current.Resources[IoC.Get<IFileTypeService>().GetFileTypeBrushName(fileItemType)] as
                        Brush,
                Stretch = Stretch.Uniform,
            };

        throw new ArgumentException("value must be IFileType");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}