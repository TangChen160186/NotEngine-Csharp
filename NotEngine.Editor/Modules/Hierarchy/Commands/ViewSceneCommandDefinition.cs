using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Results;
using NotEngine.Editor.Modules.Hierarchy.ViewModels;


namespace NotEngine.Editor.Modules.Hierarchy.Commands;
[CommandDefinition]
public class ViewHierarchyCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.Hierarchy";

    public override string Name => CommandName;

    public override string Text => "Hierarchy";

    public override string ToolTip => "Hierarchy";
}

[CommandHandler]
public class ViewHierarchyCommandHandler : CommandHandlerBase<ViewHierarchyCommandDefinition>
{
    public override async Task Run(Command command)
    {
        await Show.Tool<HierarchyViewModel>().ExecuteAsync();
    }
}