using System.ComponentModel.Composition;
using System.Windows.Controls;
using Gemini.Framework;

namespace Module.Scene.ViewModels;

[Export]
public class SceneViewModel : Document
{
    public override string DisplayName { get; set; } = "Scene";


    public SceneViewModel()
    {
       
    }

    private UserControl userControl;

}
