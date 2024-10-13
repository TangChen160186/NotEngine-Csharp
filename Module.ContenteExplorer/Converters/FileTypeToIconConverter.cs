using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Caliburn.Micro;
using Module.ContentExplorer.Core.FileType;
using Module.ContentExplorer.Servies;

namespace Module.ContentExplorer.Converters;

internal class FileTypeToIconConverter: IValueConverter
{ 


    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is IFileType fileItemType)
            return IoC.Get<IFileTypeService>().GetFileTypePath(fileItemType);

        throw new ArgumentException("value must be IFileType");

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

internal class FileTypeToIconBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IFileType fileItemType)
        {
            return Application.Current.Resources[IoC.Get<IFileTypeService>().GetFileTypeBrushName(fileItemType)] as Brush;

        }

        throw new ArgumentException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
