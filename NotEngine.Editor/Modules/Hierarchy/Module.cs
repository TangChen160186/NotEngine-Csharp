using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Menus;
using NotEngine.Editor.Modules.Hierarchy.Commands;
using NotEngine.Editor.Modules.Hierarchy.ViewModels;

namespace NotEngine.Editor.Modules.Hierarchy;

[Export(typeof(IModule))]
public class Module: ModuleBase
{
    [Export]
    public static readonly MenuItemDefinition ViewHierarchyMenuItem = new CommandMenuItemDefinition<ViewHierarchyCommandDefinition>(
        Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 0);
    public override IEnumerable<Type> DefaultTools
    {
        get
        {
            yield return typeof(HierarchyViewModel);
        }
    }
}