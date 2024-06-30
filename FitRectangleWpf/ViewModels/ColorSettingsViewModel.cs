using System.Windows.Media;

namespace FitRectangle
{
    public class ColorSettingsViewModel : ViewModelBase, IColorSetting
    {
        public ColorSettingsViewModel(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }
        public bool IsIgnore { get; set; }
    }
}