using Caliburn.Micro;
using Module.ContentExplorer.Core.FileType;

namespace Module.ContentExplorer.Core.ContextMenu;

public abstract class ContentExplorerContextMenuItemDefinition : ContentExplorerContextMenuItemDefinitionBase
{
    private readonly int _sortOrder;

    public ContentExplorerContextMenuItemGroupDefinition Group { get; private set; }

    public override int SortOrder => _sortOrder;
    public override IEnumerable<Type> FileTypes { get; }


   
    protected ContentExplorerContextMenuItemDefinition(ContentExplorerContextMenuItemGroupDefinition group, int sortOrder,IEnumerable<Type> fileTypes)
    {
        Group = group;
        _sortOrder = sortOrder;
        if (fileTypes == null)
        {
            FileTypes = IoC.GetAllInstances(typeof(IFileType)).Select(e => e.GetType());
        }
        else
        {
            FileTypes = fileTypes;
        }

     
    }
}