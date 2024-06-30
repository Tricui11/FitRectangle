using System.Windows.Media;

namespace FitRectangle
{
    public interface IColorSetting
    {
        Color Color { get; set; }
        bool IsIgnore { get; set; }
    }
}
