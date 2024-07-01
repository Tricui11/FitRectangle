using System.IO;

namespace FitRectangle
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            File.AppendAllText(_filePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
