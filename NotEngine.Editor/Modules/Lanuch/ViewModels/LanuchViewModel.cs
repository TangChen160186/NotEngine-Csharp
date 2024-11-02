using Gemini.Framework;
using System.ComponentModel.Composition;

namespace NotEngine.Editor.Modules.Lanuch.ViewModels;

[Export(typeof(LanuchViewModel))]
public class LanuchViewModel: WindowBase
{
    public string Title { get; set; } = "fafa";
}