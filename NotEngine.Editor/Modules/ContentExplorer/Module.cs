﻿using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Menus;
using NotEngine.Editor.Modules.ContentExplorer.Commands;
using NotEngine.Editor.Modules.ContentExplorer.ViewModels;

namespace NotEngine.Editor.Modules.ContentExplorer;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    [Export]
    public static readonly MenuItemDefinition ViewHomeMenuItem = new CommandMenuItemDefinition<ViewContentExplorerCommandDefinition>(
        Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 0);
    public override IEnumerable<Type> DefaultTools
    {
        get
        {
            yield return typeof(ContentExplorerViewModel);

        }
    }

    public override Task PostInitializeAsync()
    {
        Shell.ShowTool<ContentExplorerViewModel>();
        return Task.CompletedTask;
    }
}