using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;
using NotEngine.Editor.Modules.ContentExplorer.Models.ContextMenu;
using NotEngine.Editor.Services;

namespace NotEngine.Editor.Modules.ContentExplorer.ViewModels;

[Export(typeof(IContextMenuBuilder))]
public class ContextMenuBuilder : IContextMenuBuilder
{
    private readonly ICommandService _commandService;
    private readonly List<ContentExplorerContextMenuItemDefinition> _fileContextMenuItems;
    private List<ContentExplorerContextMenuItemGroupDefinition> _fileContextMenuItemGroupDefinitions;
    [ImportingConstructor]
    public ContextMenuBuilder(ICommandService commandService,
        [ImportMany] ContentExplorerContextMenuItemDefinition[] definitions,
        [ImportMany] ContentExplorerContextMenuItemGroupDefinition[] groups)
    {
        _commandService = commandService;
        _fileContextMenuItems = definitions.OrderBy(e => e.SortOrder).ToList();
        _fileContextMenuItemGroupDefinitions = groups.OrderBy(e => e.SortOrder).ToList();
    }

    public ContextMenuModel BuildMenuBar(string extension)
    {
        ContextMenuModel result = new ContextMenuModel();
        Add(null, null, result,extension);
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
    private void Add(ContentExplorerContextMenuItemDefinition? itemDefinition, StandardContextMenuItem? parentMenuItem, IObservableCollection<ContextMenuItemBase> result,
        string extension)
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
                        if ( menuItem.AssetTypes.Any(e => e == IoC.Get<IAssetTypeService>().GetAssetType(extension)))
                        {
                            result.Add(item);
                            Add(menuItem, item, item.Children,extension);
                        }
               
                    }
                }

                result.Add(new ContextMenuItemSeparator());
            }
        }
    }
}