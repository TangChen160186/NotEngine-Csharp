using System.ComponentModel.Composition;
using System.Windows.Input;
using Gemini.Framework.Commands;

namespace NotEngine.Editor.Modules.ContentExplorer.Commands;

[CommandDefinition]
public class RenameCommandDefinition : CommandDefinition
{
    public override string Name => "Rename";
    public override string Text => Name;
    public override string ToolTip => Name;

    [Export]
    public static CommandKeyboardShortcut RenameCommandKeyboardShortcut =
        new CommandKeyboardShortcut<RenameCommandDefinition>(new KeyGesture(Key.F2));
}