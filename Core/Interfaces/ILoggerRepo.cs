using System;
using System.Threading.Tasks;

namespace Services.D.Core.Interfaces
{
    public interface ILoggerRepo
    {
        Task LogInfo(string message);
        Task LogError(string message);
        Task LogError(Exception exception);
    }
}
