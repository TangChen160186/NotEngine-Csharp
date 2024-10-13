using Gemini.Framework;
using Gemini.Framework.Services;
using System.ComponentModel.Composition;

namespace NotEngine.Editor.Modules.Startup;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    [Import]
    private IResourceManager _resourceManager;
    public override void Initialize()
    {
        Shell.ShowFloatingWindowsInTaskbar = true;
        Shell.ToolBars.Visible = true;
        MainWindow.Title = "NotEngine";
        MainWindow.Icon = _resourceManager.GetBitmap("Resources/Icons/AppIcon.png", "NotEngine.Editor");
    }
}