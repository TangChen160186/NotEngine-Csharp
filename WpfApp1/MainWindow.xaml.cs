using OpenTK.Mathematics;
using OpenTK.Wpf;
using System.Windows;
using OpenTK.Graphics.OpenGL;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // [...]
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 4,
                MinorVersion = 6
            };
            OpenTkControl.Start(settings);
        }

        private void OpenTkControl_OnRender(TimeSpan obj)
        {
            GL.ClearColor(Color4.Blue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}