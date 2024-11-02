using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Gemini.Framework.Controls;
using Gemini.Framework.Utils;
using NotEngine.Editor.Modules.ContentExplorer.Models.ContextMenu;

namespace NotEngine.Editor.Modules.ContentExplorer.Controls;

public class ContextMenuItemEx : MenuItem
{
    private object _currentItem;

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        _currentItem = item;
        return base.IsItemItsOwnContainerOverride(item);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return GetContainer(this, _currentItem);
    }

    internal static DependencyObject GetContainer(FrameworkElement frameworkElement, object item)
    {
        if (item is ContextMenuItemSeparator)
            return new Separator();

        const string styleKey = "ContextMenuItem";

        var result = new ContextMenuItemEx();
        result.SetResourceReference(DynamicStyle.BaseStyleProperty, typeof(MenuItem));
        result.SetResourceReference(DynamicStyle.DerivedStyleProperty, styleKey);
        return result;
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (!(element is Separator))
        {
            ItemsControlUtility.UpdateSeparatorsVisibility(this);
            DependencyPropertyDescriptor.FromProperty(VisibilityProperty, element.GetType()).AddValueChanged(element, UpdateSeparatorsVisibility);
        }
    }

    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        if (!(element is Separator))
        {
            DependencyPropertyDescriptor.FromProperty(VisibilityProperty, element.GetType()).RemoveValueChanged(element, UpdateSeparatorsVisibility);
            ItemsControlUtility.UpdateSeparatorsVisibility(this);
        }

        base.ClearContainerForItemOverride(element, item);
    }

    private void UpdateSeparatorsVisibility(object sender, EventArgs e)
    {
        ItemsControlUtility.UpdateSeparatorsVisibility(this);
    }
}