using System.ComponentModel.Composition;
using Gemini.Framework;

namespace Module.Scene.ViewModels;

[Export]
public class SceneViewModel : Document
{
    public override string DisplayName { get; set; } = "Scene";

    protected override void OnViewReady(object view)
    {
        base.OnViewReady(view);
    }
}
