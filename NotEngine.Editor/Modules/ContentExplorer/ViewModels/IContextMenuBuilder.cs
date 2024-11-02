using NotEngine.Editor.Modules.ContentExplorer.Models.ContextMenu;

namespace NotEngine.Editor.Modules.ContentExplorer.ViewModels;

public interface IContextMenuBuilder
{
    ContextMenuModel BuildMenuBar(Type fileType);
}