using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

public class CommandContentExplorerContextMenuItemDefinition<TCommandDefinition> : ContentExplorerContextMenuItemDefinition
    where TCommandDefinition : CommandDefinitionBase
{
    private readonly CommandDefinitionBase _commandDefinition;
    private readonly KeyGesture _keyGesture;

    public override string Text => _commandDefinition.Text;
    public override string PathData => _commandDefinition.PathData;
    public override string PathDataForegroundName => _commandDefinition.PathDataForegroundName;


    public override KeyGesture KeyGesture => _keyGesture;

    public override CommandDefinitionBase CommandDefinition => _commandDefinition;

    public CommandContentExplorerContextMenuItemDefinition(ContentExplorerContextMenuItemGroupDefinition group, int sortOrder,IEnumerable<Type> fileTypes = null)
        : base(group, sortOrder, fileTypes)
    {
        _commandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(TCommandDefinition));
        _keyGesture = IoC.Get<ICommandKeyGestureService>().GetPrimaryKeyGesture(_commandDefinition);
    }
}