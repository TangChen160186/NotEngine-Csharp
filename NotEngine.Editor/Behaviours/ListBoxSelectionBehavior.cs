using System.Collections;
using System.Windows.Controls;
using System.Windows;

namespace NotEngine.Editor.Behaviours;

public static class ListBoxSelectedItemsBehavior
{
    public static readonly DependencyProperty BindableSelectedItemsProperty =
        DependencyProperty.RegisterAttached(
            "BindableSelectedItems",
            typeof(IList),
            typeof(ListBoxSelectedItemsBehavior),
            new PropertyMetadata(null, OnBindableSelectedItemsChanged));

    public static IList GetBindableSelectedItems(DependencyObject obj)
    {
        return (IList)obj.GetValue(BindableSelectedItemsProperty);
    }

    public static void SetBindableSelectedItems(DependencyObject obj, IList value)
    {
        obj.SetValue(BindableSelectedItemsProperty, value);
    }

    private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListBox listBox)
        {
            listBox.SelectionChanged -= ListBox_SelectionChanged;
            listBox.SelectionChanged += ListBox_SelectionChanged;
        }
    }

    private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            var bindableSelectedItems = GetBindableSelectedItems(listBox);
            if (bindableSelectedItems == null) return;

            bindableSelectedItems.Clear();

            foreach (var item in listBox.SelectedItems)
            {
                bindableSelectedItems.Add(item);
            }
        }
    }
}
