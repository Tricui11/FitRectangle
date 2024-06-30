using System.Windows;
using System.Windows.Input;

namespace RectangleApp
{
    public partial class MainWindow : Window
    {
        private Point _origin;
        private Point _start;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _start = e.GetPosition(this);
            _origin.X = canvasTranslateTransform.X;
            _origin.Y = canvasTranslateTransform.Y;
            Cursor = Cursors.Hand;
            canvas.CaptureMouse();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            canvas.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                Vector v = _start - e.GetPosition(this);
                canvasTranslateTransform.X = _origin.X - v.X;
                canvasTranslateTransform.Y = _origin.Y - v.Y;
            }
        }
    }
}
