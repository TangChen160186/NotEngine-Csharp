using Module.ContentExplorer.Models.ContextMenu;

namespace Module.ContentExplorer.ViewModels;

public interface IContextMenuBuilder
{
    ContextMenuModel BuildMenuBar();
}