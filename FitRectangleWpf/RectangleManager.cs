using System.Windows.Media;

namespace FitRectangle
{
    public class RectangleManager
    {
        public Rectangle MainRectangle { get; private set; }
        public List<Rectangle> SecondaryRectangles { get; private set; }
        private Action<string> LogAction;

        public RectangleManager(Rectangle mainRectangle, List<Rectangle> secondaryRectangles, Action<string> logAction)
        {
            MainRectangle = mainRectangle;
            SecondaryRectangles = secondaryRectangles;
            LogAction = logAction;
        }

        public void UpdateMainRectangle(bool ignoreOutside, IEnumerable<ColorSettingsViewModel> colorsSettings)
        {
            var SecondaryRectanglesToCalculate = SecondaryRectangles
                .Where(r => colorsSettings.Where(x => !x.IsIgnore).Select(x => x.Color).Contains(r.Color));
            double minX = !ignoreOutside
                ? SecondaryRectanglesToCalculate.Min(r => r.BotLeft.X)
                : SecondaryRectanglesToCalculate.Where(x => x.BotLeft.X >= MainRectangle.BotLeft.X).Min(r => r.BotLeft.X);
            double minY = !ignoreOutside
                ? SecondaryRectanglesToCalculate.Min(r => r.BotLeft.Y)
                : SecondaryRectanglesToCalculate.Where(x => x.BotLeft.Y >= MainRectangle.BotLeft.Y).Min(r => r.BotLeft.Y);
            double maxX = !ignoreOutside
                ? SecondaryRectanglesToCalculate.Max(r => r.TopRight.X)
                : SecondaryRectanglesToCalculate.Where(x => x.TopRight.X <= MainRectangle.TopRight.X).Max(r => r.TopRight.X);
            double maxY = !ignoreOutside
                ? SecondaryRectanglesToCalculate.Max(r => r.TopRight.Y)
                : SecondaryRectanglesToCalculate.Where(x => x.TopRight.Y <= MainRectangle.TopRight.Y).Max(r => r.TopRight.Y);

            MainRectangle.BotLeft = new Point(minX, minY);
            MainRectangle.TopRight = new Point(maxX, maxY);

            LogAction($"Main rectangle updated: BotLeft({minX}, {minY}), TopRight({maxX}, {maxY})");
        }

        public List<Rectangle> FilterRectangles(IEnumerable<Color> colors)
        {
            var filteredRectangles = SecondaryRectangles
                .ToList();

            LogAction($"Filtered {filteredRectangles.Count} rectangles");
            return filteredRectangles;
        }
    }
}
