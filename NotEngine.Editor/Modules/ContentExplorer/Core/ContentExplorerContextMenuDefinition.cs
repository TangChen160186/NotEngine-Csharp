using System.ComponentModel.Composition;
using NotEngine.Editor.Models;
using NotEngine.Editor.Modules.ContentExplorer.Commands;
using NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

namespace NotEngine.Editor.Modules.ContentExplorer.Core;

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

    [Export]
    public static ContentExplorerContextMenuItemGroupDefinition CreateGroup =
        new ContentExplorerContextMenuItemGroupDefinition(1);

    [Export]
    public static ContentExplorerContextMenuItemDefinition Create =
        new TextContentExplorerContextMenuItemDefinition(CreateGroup, 0, "Create", [AssetType.Folder]);

    [Export]
    public static ContentExplorerContextMenuItemGroupDefinition CreateSubGroup =
        new ContentExplorerContextMenuItemGroupDefinition(0, Create);

    [Export]
    public static ContentExplorerContextMenuItemDefinition CreateFolder =
        new CommandContentExplorerContextMenuItemDefinition<CreateFolderCommandDefinition>(CreateSubGroup, 0);


    [Export]
    public static ContentExplorerContextMenuItemGroupDefinition ImportGroup =
        new ContentExplorerContextMenuItemGroupDefinition(2);
    [Export]
    public static ContentExplorerContextMenuItemDefinition Import =
        new TextContentExplorerContextMenuItemDefinition(ImportGroup,0,"Import", [AssetType.Folder]);

    [Export]
    public static ContentExplorerContextMenuItemGroupDefinition ImportSubGroup =
        new ContentExplorerContextMenuItemGroupDefinition(0, Import);
    [Export]
    public static ContentExplorerContextMenuItemDefinition ImportTexture =
        new CommandContentExplorerContextMenuItemDefinition<ImportTextureCommandDefinition>(ImportSubGroup, 0);


}