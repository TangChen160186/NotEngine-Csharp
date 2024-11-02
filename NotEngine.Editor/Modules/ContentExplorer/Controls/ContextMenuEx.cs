using System.Windows;
using System.Windows.Input;

namespace NotEngine.Editor.Modules.ContentExplorer.Controls
{
    public class ContextMenuEx : System.Windows.Controls.ContextMenu
    {
       
        private object _currentItem;

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            _currentItem = item;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return ContextMenuItemEx.GetContainer(this, _currentItem);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            UpdateVisibility();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            UpdateVisibility();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            UpdateVisibility();
        }

        private static void AutoHidePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var menu = (ContextMenuEx)dependencyObject;
            menu.UpdateVisibility();
        }

        private void UpdateVisibility()
        {

            if (IsKeyboardFocused || IsFocused || IsKeyboardFocusWithin)
            {
                Height = double.NaN;
            }
            else
            {
                Height = 0;
            }
        }
    }
}
