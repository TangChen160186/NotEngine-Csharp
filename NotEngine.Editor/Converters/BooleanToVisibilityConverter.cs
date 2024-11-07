using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using NotEngine.Editor.Services;

namespace NotEngine.Editor.Converters;

class BooleanToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!(value is bool flag))
            throw new ArgumentException();
        return flag ? Visibility.Visible : Visibility.Hidden;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

class ExtensionToAssetTypeConverter : IValueConverter
{   
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string extension)
        {
            IoC.Get<IAssetTypeService>().GetAssetType(extension);
        }

        throw new ArgumentOutOfRangeException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new ArgumentOutOfRangeException();
    }
}