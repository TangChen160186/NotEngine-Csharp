using System.ComponentModel.Composition;
using Module.ContentExplorer.Commands;
using Module.ContentExplorer.Core.ContextMenu;

namespace Module.ContentExplorer.Core;

public class ContentExplorerContextMenuDefinition
{
    [Export]
    public static ContentExplorerContextMenuItemGroupDefinition BaseGroup =
        new ContentExplorerContextMenuItemGroupDefinition(0);
    [Export]
    public static ContentExplorerContextMenuItemDefinition Rename =
        new CommandContentExplorerContextMenuItemDefinition<RenameCommandDefinition>(BaseGroup, 0);

    [Export]
    public static ContentExplorerContextMenuItemDefinition OpenInFileExplorer =
        new CommandContentExplorerContextMenuItemDefinition<OpenInFileExplorerCommandDefinition>(BaseGroup, 1);
}