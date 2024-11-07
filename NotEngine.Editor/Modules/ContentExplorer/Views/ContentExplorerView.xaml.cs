using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NotEngine.Editor.Modules.ContentExplorer.Views;

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

    private void MyListBox_Loaded(object sender, RoutedEventArgs e)
    {
        AdjustItemWidths();
    }

    private void MyListBox_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        AdjustItemWidths();
    }

    private void AdjustItemWidths()
    {
        double minItemWidth = 120; // 设置每个项目的最小宽度
        double listBoxWidth = MyListBox.ActualWidth;

        if (listBoxWidth > 0 && minItemWidth > 0)
        {
            // 动态计算每行可以容纳的项目数量
            int itemsPerRow = Math.Max(1, (int)(listBoxWidth / minItemWidth));
            double itemWidth = listBoxWidth / itemsPerRow;

            // 查找 ListBox 的 WrapPanel 并设置 ItemWidth
            WrapPanel wrapPanel = GetWrapPanel(MyListBox);
            if (wrapPanel != null)
            {
                wrapPanel.ItemWidth = itemWidth;
            }
        }
    }

    // 递归查找 ListBox 的 WrapPanel
    private WrapPanel GetWrapPanel(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is WrapPanel panel)
                return panel;

            var result = GetWrapPanel(child);
            if (result != null)
                return result;
        }
        return null;
    }
}
