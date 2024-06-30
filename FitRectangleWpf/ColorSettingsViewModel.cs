using System.Windows.Media;

public class ColorSettingsViewModel : ViewModelBase
{
    public ColorSettingsViewModel(Color color)
    {
        Color = color;
    }

    public Color Color { get; set; }
    public bool IsIgnore { get; set; }
}
