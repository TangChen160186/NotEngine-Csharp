using System.Windows.Controls;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Wpf;

namespace NotEngine.Editor.Modules.Scene.Views
{
    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl
    {
        public SceneView()
        {

            InitializeComponent();
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 4,
                MinorVersion = 6
            };
            OpenTkControl.Start(settings);
        
        }
        private unsafe void OpenTkControl_OnRender(TimeSpan obj)
        {
            int a;
            GL.GetIntegerv(GetPName.DrawFramebufferBinding,&a);
            GL.ClearColor(new Color4<Rgba>(64 / 255f, 0, 127 / 255f, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}
