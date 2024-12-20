﻿using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Menus;
using NotEngine.Editor.Modules.Scene.Commands;
using NotEngine.Editor.Modules.Scene.ViewModels;

namespace NotEngine.Editor.Modules.Scene;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    [Export]
    public static readonly MenuItemDefinition ViewHomeMenuItem = new CommandMenuItemDefinition<ViewSceneCommandDefinition>(
        Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 0);

    public override IEnumerable<IDocument> DefaultDocuments
    {
        get
        {
            yield return IoC.Get<SceneViewModel>();
        }
    }

    public override Task PostInitializeAsync()
    {
        return Shell.OpenDocumentAsync(IoC.Get<SceneViewModel>());
    }
}