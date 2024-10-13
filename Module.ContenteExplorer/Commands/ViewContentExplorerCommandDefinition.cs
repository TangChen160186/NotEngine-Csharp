using Gemini.Framework.Commands;

namespace Module.ContentExplorer.Commands;

[CommandDefinition]
public class ViewContentExplorerCommandDefinition: CommandDefinition
{
    public const string CommandName = "View.ContentExplorer";

    public override string Name => CommandName;

    public override string Text => "ContentExplorer";

    public override string ToolTip => "ContentExplorer";
}