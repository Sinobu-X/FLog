using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLog.Handlers
{
    public interface ILogHandler
    {
        LogLevel Level{ get; set; } 
        Task<bool> Write(LogRepository repository, List<LogData> items);
    }
}