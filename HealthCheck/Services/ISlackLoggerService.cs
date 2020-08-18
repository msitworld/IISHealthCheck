using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HealthCheck.Services
{
    public interface ISlackLoggerService
    {
        Task LogAsync(LogLevel level, string message);
    }
}
