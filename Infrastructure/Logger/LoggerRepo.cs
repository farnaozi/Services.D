
using System;
using System.Threading.Tasks;
using Services.D.Core.Interfaces;
using NLog;

namespace Services.D.Infrastructure.Logger
{
    public class LoggerRepo : ILoggerRepo
    {
        private readonly NLog.Logger _logger;

        public LoggerRepo()
        {
            _logger = LogManager.GetLogger("*");
        }
        public Task LogInfo(string message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine(message);
                _logger.Info(message);
            });
        }
        public Task LogError(string message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine(message);
                _logger.Error(message);
            });
        }
        public Task LogError(Exception exception)
        {
            return Task.Run(() =>
            {
                var message = exception.Message;

                if (exception.InnerException != null)
                {
                    message = $"{message}, {exception.InnerException.Message}";
                }

                Console.WriteLine(message);
                _logger.Error(message);
            });
        }
    }
}