using System.Windows.Input;

namespace NotEngine.Editor.Modules.ContentExplorer.Models.ContextMenu;    

public abstract class StandardContextMenuItem : ContextMenuItemBase
{
    public abstract string Text { get; }
    public abstract string PathData { get; }
    public abstract string PathDataForegroundName { get; }
    public abstract string InputGestureText { get; }
    public abstract ICommand Command { get; }
    public abstract bool IsChecked { get; }
    public abstract bool IsVisible { get; }
}
