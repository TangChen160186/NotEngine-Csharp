using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using NotEngine.Editor.Models;

namespace NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

public class CommandContentExplorerContextMenuItemDefinition<TCommandDefinition> : ContentExplorerContextMenuItemDefinition
    where TCommandDefinition : CommandDefinitionBase
{
    private readonly CommandDefinitionBase _commandDefinition;

    public override string Text => _commandDefinition.Text;
    public override string PathData => _commandDefinition.PathData;
    public override string PathDataForegroundName => _commandDefinition.PathDataForegroundName;


    public override KeyGesture KeyGesture { get; }

    public override CommandDefinitionBase CommandDefinition => _commandDefinition;

    public CommandContentExplorerContextMenuItemDefinition(ContentExplorerContextMenuItemGroupDefinition group, int sortOrder,IEnumerable<AssetType> fileTypes = null)
        : base(group, sortOrder, fileTypes)
    {
        _commandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(TCommandDefinition));
        KeyGesture = IoC.Get<ICommandKeyGestureService>().GetPrimaryKeyGesture(_commandDefinition);
    }
}