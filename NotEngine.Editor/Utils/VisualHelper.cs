using System.Windows;
using System.Windows.Media;

namespace NotEngine.Editor.Utils;
public static class VisualHelper
{
    // 泛型方法，根据控件类型查找子控件
    public static T? FindChild<T>(DependencyObject parent, string? childName = null) where T : DependencyObject
    {
        // 检查 parent 是否为空
        if (parent == null) return null;

        // 遍历子控件
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            // 如果子控件是目标类型
            if (child is T foundChild)
            {
                // 如果指定了控件名称，进一步检查控件的名称
                if (!string.IsNullOrEmpty(childName))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        return foundChild;
                    }
                }
                else
                {
                    // 未指定控件名称，直接返回找到的控件
                    return foundChild;
                }
            }

            // 递归在子控件中查找
            T? childOfChild = FindChild<T>(child, childName);
            if (childOfChild != null)
            {
                return childOfChild;
            }
        }

        return null; // 没有找到
    }
}