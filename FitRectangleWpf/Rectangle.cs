namespace FitRectangle
{
    public class Rectangle
    {
        public Point BotLeft { get; set; }
        public Point TopRight { get; set; }
        public string Color { get; set; }

        public Rectangle(Point botLeft, Point topRight, string color)
        {
            BotLeft = botLeft;
            TopRight = topRight;
            Color = color;
        }
    }
}
