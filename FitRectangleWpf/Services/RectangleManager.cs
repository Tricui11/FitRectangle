namespace FitRectangle
{
    public class RectangleManager
    {
        private readonly ILogger _logger;

        public RectangleManager(Root root, ILogger logger)
        {
            _logger = logger;
            Root = root;
        }

        public Root Root { get; private set; }

        public void UpdateMainRectangle(bool ignoreOutside, IEnumerable<IColorSetting> colorsSettings)
        {
            var allowedColors = colorsSettings.Where(x => !x.IsIgnore).Select(x => x.Color);
            var secondaryRectanglesToCalculate = Root.SecondaryRectangles
                .Where(r => allowedColors.Contains(r.Color));
            double minX = !ignoreOutside
                ? secondaryRectanglesToCalculate.Min(r => r.BotLeft.X)
                : secondaryRectanglesToCalculate.Where(x => x.BotLeft.X >= Root.MainRectangle.BotLeft.X).Min(r => r.BotLeft.X);
            double minY = !ignoreOutside
                ? secondaryRectanglesToCalculate.Min(r => r.BotLeft.Y)
                : secondaryRectanglesToCalculate.Where(x => x.BotLeft.Y >= Root.MainRectangle.BotLeft.Y).Min(r => r.BotLeft.Y);
            double maxX = !ignoreOutside
                ? secondaryRectanglesToCalculate.Max(r => r.TopRight.X)
                : secondaryRectanglesToCalculate.Where(x => x.TopRight.X <= Root.MainRectangle.TopRight.X).Max(r => r.TopRight.X);
            double maxY = !ignoreOutside
                ? secondaryRectanglesToCalculate.Max(r => r.TopRight.Y)
                : secondaryRectanglesToCalculate.Where(x => x.TopRight.Y <= Root.MainRectangle.TopRight.Y).Max(r => r.TopRight.Y);

            Root.MainRectangle.BotLeft = new Point(minX, minY);
            Root.MainRectangle.TopRight = new Point(maxX, maxY);
            
            _logger.Log($"Main rectangle updated: BotLeft({minX}, {minY}), TopRight({maxX}, {maxY})");
        }
    }
}
