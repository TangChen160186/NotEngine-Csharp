using System.Windows.Input;
using Gemini.Framework.Commands;
using NotEngine.Editor.Models;

namespace NotEngine.Editor.Modules.ContentExplorer.Core.ContextMenu;

[System.Diagnostics.DebuggerDisplay("{Text}")]
public abstract class ContentExplorerContextMenuItemDefinitionBase
{
    public abstract int SortOrder { get; }
    public abstract string Text { get; }
    public abstract string PathData { get; }
    public abstract string PathDataForegroundName { get; }
    public abstract KeyGesture KeyGesture { get; }
    public abstract CommandDefinitionBase CommandDefinition { get; }

    public abstract IEnumerable<AssetType> AssetTypes { get; }
}