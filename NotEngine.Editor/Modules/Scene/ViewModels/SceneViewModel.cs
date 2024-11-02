using System.ComponentModel.Composition;
using Gemini.Framework;

namespace NotEngine.Editor.Modules.Scene.ViewModels;

[Export]
public class SceneViewModel : Document
{
    public override string DisplayName { get; set; } = "Scene";

    protected override void OnViewReady(object view)
    {
        base.OnViewReady(view);
    }
}
