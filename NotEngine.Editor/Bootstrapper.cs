using System.IO;
using System.Windows;
using Caliburn.Micro;
using Gemini;

namespace NotEngine.Editor
{
    public class Bootstrapper: AppBootstrapper
    {
        private IWindowManager _windowManager;
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length < 1)
            {
                Application.Current.Shutdown();
            }
            ProjectInfo.ProjectPath = e.Args[0];
            ProjectInfo.AssetPath = Path.Combine(ProjectInfo.ProjectPath,"Assets");
            ProjectInfo.DefaultAssetPath = Path.Combine(ProjectInfo.ProjectPath, "Defaults");
            ProjectInfo.LogPath = Path.Combine(ProjectInfo.ProjectPath, "Logs");
            base.OnStartup(sender, e);
        }
    }
}
