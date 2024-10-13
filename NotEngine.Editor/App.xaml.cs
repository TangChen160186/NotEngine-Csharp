using System.Windows;

namespace NotEngine.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // TODO 
        public static string ProjectPath = @"C:\Users\16018\Desktop\TestPro";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //ProjectPath = e.Args[1];
        }
    }

}
