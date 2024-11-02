using Gemini.Framework.Commands;
using Gemini.Framework.Results;
using NotEngine.Editor.Modules.ContentExplorer.ViewModels;

namespace NotEngine.Editor.Modules.ContentExplorer.Commands;

[CommandHandler]
public class ViewContentExplorerCommandHandler : CommandHandlerBase<ViewContentExplorerCommandDefinition>
{
    public override async Task Run(Command command)
    {
        Show.Tool<ContentExplorerViewModel>();
    }
}