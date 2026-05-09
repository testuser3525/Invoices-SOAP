using System;
using System.IO;

namespace Invoices.Services.Logging
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logDirectory;

        public FileLoggerService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            _logDirectory = Path.Combine(baseDir, "logs");

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public void Info(string source, string message)
        {
            string entry =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{source}] INFO: {message}{Environment.NewLine}";

            Write(entry);
        }

        public void Error(string source, string operation, Exception ex)
        {
            string entry =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{source}] ERROR in {operation}: {ex.Message}{Environment.NewLine}";

            Write(entry);
        }

        private void Write(string entry)
        {
            string filePath = Path.Combine(
                _logDirectory,
                $"invoices-{DateTime.Now:yyyy-MM-dd}.log"
            );

            File.AppendAllText(filePath, entry);
        }
    }
}