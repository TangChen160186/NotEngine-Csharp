﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Editor.Common.Converters;

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