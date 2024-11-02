using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Results;
using NotEngine.Editor.Modules.Scene.ViewModels;

namespace NotEngine.Editor.Modules.Scene.Commands;
[CommandDefinition]
public class ViewSceneCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.Scene";

    public override string Name => CommandName;

    public override string Text => "Scene";

    public override string ToolTip => "Scene";
}

[CommandHandler]
public class ViewSceneCommandHandler : CommandHandlerBase<ViewSceneCommandDefinition>
{
    public override async Task Run(Command command)
    {
        await Show.Document<SceneViewModel>().ExecuteAsync();
    }
}