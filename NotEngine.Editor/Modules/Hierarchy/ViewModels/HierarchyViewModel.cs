using Gemini.Framework;
using System.ComponentModel.Composition;
using Gemini.Framework.Services;

namespace NotEngine.Editor.Modules.Hierarchy.ViewModels;

[Export]
public class HierarchyViewModel: Tool
{
    public override PaneLocation PreferredLocation { get; } = PaneLocation.Left;

    public override string DisplayName { get; set; } = "Hierarchy";
}