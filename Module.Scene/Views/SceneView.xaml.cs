﻿using OpenTK.Mathematics;
using OpenTK.Wpf;
using System.Windows.Controls;
using OpenTK.Graphics.OpenGL;

namespace Module.Scene.Views
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
                MajorVersion = 3,
                MinorVersion = 5
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
