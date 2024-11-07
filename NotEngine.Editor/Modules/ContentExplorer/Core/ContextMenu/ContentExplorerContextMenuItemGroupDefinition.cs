namespace NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

public class ContentExplorerContextMenuItemGroupDefinition(
    int sortOrder,
    ContentExplorerContextMenuItemDefinitionBase? parent = null)
{
    public ContentExplorerContextMenuItemDefinitionBase? Parent { get; private set; } = parent;

    public int SortOrder => sortOrder;
}