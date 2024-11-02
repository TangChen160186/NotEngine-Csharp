using Gemini.Framework.Commands;

namespace NotEngine.Editor.Modules.ContentExplorer.Commands;

[CommandDefinition]
public class ImportTextureCommandDefinition : CommandDefinition
{
    public override string Name => "Texture";
    public override string Text => Name;
    public override string ToolTip => Name;
}