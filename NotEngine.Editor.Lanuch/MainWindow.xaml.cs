using MahApps.Metro.Controls;
using NotEngine.Editor.Lanuch.ViewModels;

namespace NotEngine.Editor.Lanuch
{
    public partial class MainWindow: MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();
        }

    }
}