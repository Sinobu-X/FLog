using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FLog.Handlers
{
    public class LogDebugHandler : ILogHandler
    {
        public LogLevel Level{ get; set; } = LogLevel.Information;
        public LogFormatter Formatter{ get; set; }
        public string FormatterString{ get; set; }

        public Task<bool> Write(LogRepository repository, List<LogData> items){
            var sb = new StringBuilder();
            foreach (var item in items){
                sb.Append(LogHelper.BuildLog(Formatter, FormatterString, repository, item));
            }

            System.Diagnostics.Debug.Write(sb.ToString());
            return Task.FromResult(true);
        }
    }
}