using Gemini.Framework.Commands;

namespace NotEngine.Editor.Modules.ContentExplorer.Commands;

[CommandDefinition]
public class CreateFolderCommandDefinition : CommandDefinition
{
    public override string Name => "Folder";
    public override string Text => Name;
    public override string ToolTip => Name;
}