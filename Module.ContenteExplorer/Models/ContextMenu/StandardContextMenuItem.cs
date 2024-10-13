using System.Windows.Input;

namespace Module.ContentExplorer.Models.ContextMenu;    

public abstract class StandardContextMenuItem : ContextMenuItemBase
{
    public abstract string Text { get; }
    public abstract Uri IconSource { get; }
    public abstract string InputGestureText { get; }
    public abstract ICommand Command { get; }
    public abstract bool IsChecked { get; }
    public abstract bool IsVisible { get; }
}
