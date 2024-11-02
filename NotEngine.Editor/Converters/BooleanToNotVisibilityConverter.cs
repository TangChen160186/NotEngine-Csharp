using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotEngine.Editor.Converters;

class BooleanToNotVisibilityConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!(value is bool flag) )
            throw new ArgumentException();
        return flag ? Visibility.Hidden: Visibility.Visible;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

