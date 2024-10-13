using System.Globalization;
using System.Windows.Input;
using Module.ContentExplorer.Core.ContextMenu;

namespace Module.ContentExplorer.Models.ContextMenu;

public class TextContextMenuItem : StandardContextMenuItem
{
    private readonly ContentExplorerContextMenuItemDefinitionBase _menuItemDefinition;

    public override string Text
    {
        get { return _menuItemDefinition.Text; }
    }

    public override string PathData => _menuItemDefinition.PathData;
    public override string PathDataForegroundName => _menuItemDefinition.PathDataForegroundName;


    public override string InputGestureText
    {
        get
        {
            return _menuItemDefinition.KeyGesture == null
                ? string.Empty
                : _menuItemDefinition.KeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
        }
    }

    public override ICommand Command
    {
        get { return null; }
    }

    public override bool IsChecked
    {
        get { return false; }
    }

    public override bool IsVisible
    {
        get { return true; }
    }

    public TextContextMenuItem(ContentExplorerContextMenuItemDefinitionBase menuItemDefinition)
    {
        _menuItemDefinition = menuItemDefinition;
    }
}