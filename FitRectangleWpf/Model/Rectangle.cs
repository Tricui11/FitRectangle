﻿using System.Windows.Media;

namespace FitRectangle
{
    public class Rectangle
    {
        public Rectangle(Point botLeft, Point topRight, Color color)
        {
            BotLeft = botLeft;
            TopRight = topRight;
            Color = color;
        }

        public Point BotLeft { get; set; }
        public Point TopRight { get; set; }
        public Color Color { get; set; }
    }
}
