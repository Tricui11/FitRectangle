using System.Windows.Controls;
using System.Windows.Shapes;

public static class ShapeExtensions
{
    public static T SetCanvasPosition<T>(this T shape, double left, double top) where T : Shape
    {
        Canvas.SetLeft(shape, left);
        Canvas.SetTop(shape, top);
        return shape;
    }
}