using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Module.ContentExplorer.Views;

public partial class ContentExplorerView : UserControl
{
    public ContentExplorerView()
    {
        InitializeComponent();
    }

    private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
        if (treeViewItem != null)
        {
            treeViewItem.Focus();
            e.Handled = true;
        }
    }

    static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
    {
        while (source != null && source.GetType() != typeof(T))
            source = VisualTreeHelper.GetParent(source);
            
        return source;
    }
}
