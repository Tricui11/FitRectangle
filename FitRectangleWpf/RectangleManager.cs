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

        public void UpdateMainRectangle()
        {
            double minX = SecondaryRectangles.Min(r => r.BotLeft.X);
            double minY = SecondaryRectangles.Min(r => r.BotLeft.Y);
            double maxX = SecondaryRectangles.Max(r => r.TopRight.X);
            double maxY = SecondaryRectangles.Max(r => r.TopRight.Y);

            MainRectangle.BotLeft = new Point(minX, minY);
            MainRectangle.TopRight = new Point(maxX, maxY);

            LogAction($"Main rectangle updated: BotLeft({minX}, {minY}), TopRight({maxX}, {maxY})");
        }

        public void FilterRectangles(bool ignoreOutside, List<string> colors, bool includeColors)
        {
            var filteredRectangles = SecondaryRectangles.Where(r =>
                (!ignoreOutside || (r.BotLeft.X >= MainRectangle.BotLeft.X && r.TopRight.X <= MainRectangle.TopRight.X &&
                                    r.BotLeft.Y >= MainRectangle.BotLeft.Y && r.TopRight.Y <= MainRectangle.TopRight.Y)) &&
                (includeColors ? colors.Contains(r.Color) : !colors.Contains(r.Color))
            ).ToList();

            SecondaryRectangles = filteredRectangles;

            LogAction($"Filtered {SecondaryRectangles.Count} rectangles");
        }
    }
}
