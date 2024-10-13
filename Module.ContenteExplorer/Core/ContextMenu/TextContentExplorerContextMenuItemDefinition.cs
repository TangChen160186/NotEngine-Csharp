using System.Windows.Input;
using Gemini.Framework.Commands;

namespace Module.ContentExplorer.Core.ContextMenu;

public class TextContentExplorerContextMenuItemDefinition : ContentExplorerContextMenuItemDefinition
{
    private readonly string _text;
    private readonly Uri _iconSource;
    public override string Text => _text;

    public override string PathData { get; }
    public override string PathDataForegroundName { get; }

    public override KeyGesture KeyGesture => null;

    public override CommandDefinitionBase CommandDefinition => null;


    public TextContentExplorerContextMenuItemDefinition(ContentExplorerContextMenuItemGroupDefinition group, int sortOrder, string text,
        string pathData = null,string pathDataForegroundName = null,IEnumerable<Type> fileTypes = null)
        : base(group, sortOrder, fileTypes)
    {
        _text = text;
        PathDataForegroundName = pathDataForegroundName;
        PathData = pathData;
    }
}