using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Module.ContentExplorer.Core.ContextMenu;
using Module.ContentExplorer.Core.FileType;
using Module.ContentExplorer.Models.ContextMenu;

namespace Module.ContentExplorer.ViewModels;

[Export(typeof(IContextMenuBuilder))]
public class ContextMenuBuilder : IContextMenuBuilder
{
    private readonly ICommandService _commandService;
    private readonly List<ContentExplorerContextMenuItemDefinition> _fileContextMenuItems;
    private List<ContentExplorerContextMenuItemGroupDefinition> _fileContextMenuItemGroupDefinitions;
    private Type _fileType;
    [ImportingConstructor]
    public ContextMenuBuilder(ICommandService commandService,
        [ImportMany] ContentExplorerContextMenuItemDefinition[] definitions,
        [ImportMany] ContentExplorerContextMenuItemGroupDefinition[] groups)
    {
        _commandService = commandService;
        _fileContextMenuItems = definitions.OrderBy(e => e.SortOrder).ToList();
        _fileContextMenuItemGroupDefinitions = groups.OrderBy(e => e.SortOrder).ToList();
    }

    public ContextMenuModel BuildMenuBar(Type fileType)
    {
        _fileType = fileType;
        ContextMenuModel result = new ContextMenuModel();
        Add(null, null, result);
        RemoveLastSeparator(result);
        return result;
    }

    private void RemoveLastSeparator(IObservableCollection<ContextMenuItemBase> result)
    {
        if(result.Count == 0) return;
        result.RemoveAt(result.Count-1);
        foreach (var item in result)
        {
            RemoveLastSeparator(item.Children);
        }
      
    }
    private void Add(ContentExplorerContextMenuItemDefinition? itemDefinition, StandardContextMenuItem? parentMenuItem, IObservableCollection<ContextMenuItemBase> result)
    {
        foreach (var group in _fileContextMenuItemGroupDefinitions)
        {
            if (group.Parent == itemDefinition)
            {
                foreach (var menuItem in _fileContextMenuItems)
                {
                    if (menuItem.Group == group)
                    {

                        var item = menuItem.CommandDefinition != null ?
                            new CommandContextMenuItem(_commandService.GetCommand(menuItem.CommandDefinition), parentMenuItem) :
                            (StandardContextMenuItem)new TextContextMenuItem(menuItem);
                        if ( menuItem.FileTypes.Any(e => e == _fileType))
                        {
                            result.Add(item);
                            Add(menuItem, item, item.Children);
                        }
               
                    }
                }

                result.Add(new ContextMenuItemSeparator());
            }
        }
    }
}