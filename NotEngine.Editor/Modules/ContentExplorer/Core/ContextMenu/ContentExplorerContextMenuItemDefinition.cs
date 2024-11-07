using Gemini.Framework.Commands;
using NotEngine.Editor.Models;
using System.Windows.Input;

namespace NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

public abstract class ContentExplorerContextMenuItemDefinition : ContentExplorerContextMenuItemDefinitionBase
{
    private readonly int _sortOrder;

    public ContentExplorerContextMenuItemGroupDefinition Group { get; private set; }
    public override int SortOrder => _sortOrder;
    public override IEnumerable<AssetType> AssetTypes { get; } = null;
    public override string PathData { get; } = null;
    public override string PathDataForegroundName { get; } = null;

    public override KeyGesture KeyGesture => null;

    public override CommandDefinitionBase CommandDefinition => null;
    protected ContentExplorerContextMenuItemDefinition(ContentExplorerContextMenuItemGroupDefinition group, int sortOrder,IEnumerable<AssetType>? assetTypes)
    {
        Group = group;
        _sortOrder = sortOrder;
        AssetTypes = assetTypes ?? Enum.GetValues(typeof(AssetType)).Cast<AssetType>();
    }
}