namespace Module.ContentExplorer.Core.ContextMenu;

public abstract class ContentExplorerContextMenuItemDefinition : ContentExplorerContextMenuItemDefinitionBase
{
    private readonly int _sortOrder;

    public ContentExplorerContextMenuItemGroupDefinition Group { get; private set; }

    public override int SortOrder => _sortOrder;

    protected ContentExplorerContextMenuItemDefinition(ContentExplorerContextMenuItemGroupDefinition group, int sortOrder)
    {
        Group = group;
        _sortOrder = sortOrder;
    }
}