namespace NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

public class ContentExplorerContextMenuItemGroupDefinition
{
    private readonly int _sortOrder;

    public ContentExplorerContextMenuItemDefinitionBase? Parent { get; private set; }

    public int SortOrder => _sortOrder;

    public ContentExplorerContextMenuItemGroupDefinition(ContentExplorerContextMenuItemDefinitionBase? parent, int sortOrder)
    {
        Parent = parent;
        _sortOrder = sortOrder;
    }
    public ContentExplorerContextMenuItemGroupDefinition(int sortOrder)
    {
        Parent = null;
        _sortOrder = sortOrder;
    }

}