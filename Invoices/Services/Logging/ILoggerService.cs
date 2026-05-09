using System;

namespace Invoices.Services.Logging
{
    public interface ILoggerService
    {
        void Info(string source, string message);
        void Error(string source, string operation, Exception ex);
    }
}