using System.Windows;

namespace FitRectangle
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            MessageBox.Show(message);
        }
    }
}
