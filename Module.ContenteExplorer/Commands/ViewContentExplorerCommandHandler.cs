using Gemini.Framework.Commands;
using Gemini.Framework.Results;
using Module.ContentExplorer.ViewModels;

namespace Module.ContentExplorer.Commands;

[CommandHandler]
public class ViewContentExplorerCommandHandler : CommandHandlerBase<ViewContentExplorerCommandDefinition>
{
    public override async Task Run(Command command)
    {
        Show.Tool<ContentExplorerViewModel>();
    }
}