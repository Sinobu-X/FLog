using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLog.Handlers
{
    public interface ILogHandler
    {
        LogLevel Level{ get; set; }
        List<string> Includes{ get; set; }
        List<string> Excludes{ get; set; }
        LogFormatter Formatter{ get; set; }
        string FormatterString{ get; set; }
        Task<bool> Write(List<LogData> items);
    }
}