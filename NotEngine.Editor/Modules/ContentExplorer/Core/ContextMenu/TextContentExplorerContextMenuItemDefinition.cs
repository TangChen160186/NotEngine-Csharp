using NotEngine.Editor.Models;

namespace NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

public class TextContentExplorerContextMenuItemDefinition(
    ContentExplorerContextMenuItemGroupDefinition group,
    int sortOrder,
    string text,
    IEnumerable<AssetType> fileTypes = null)
    : ContentExplorerContextMenuItemDefinition(group, sortOrder, fileTypes)
{
    public override string Text { get; } = text;
}