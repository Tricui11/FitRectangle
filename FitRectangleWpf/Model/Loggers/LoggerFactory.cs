namespace FitRectangle
{
    public static class LoggerFactory
    {
        public static ILogger CreateLogger(bool isFileLogging, string filePath)
        {
            if (isFileLogging)
            {
                return new FileLogger(filePath);
            }
            else
            {
                return new ConsoleLogger();
            }
        }
    }
}
