using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FLog.Handlers
{
    public class LogConsoleHandler : ILogHandler
    {
        public LogLevel Level{ get; set; } = LogLevel.Information;
        public LogLevel MaxLevel{ get; set; } = LogLevel.Critical;
        public LogFormatter Formatter{ get; set; }
        public string FormatterString{ get; set; }
        public List<string> Includes{ get; set; }
        public List<string> Excludes{ get; set; }

        public Task<bool> Write(List<LogData> items){
            var sb = new StringBuilder();
            var isFirst = true;
            var lastLevel = LogLevel.None;

            foreach (var item in items){
                if (isFirst){
                    isFirst = false;
                }
                else{
                    if (lastLevel != item.Level){
                        WriteInner(lastLevel, sb.ToString());
                        sb.Clear();
                    }
                }

                lastLevel = item.Level;
                sb.Append(LogHelper.BuildLog(Formatter, FormatterString, item));
            }

            WriteInner(lastLevel, sb.ToString());
            return Task.FromResult(true);
        }

        private void WriteInner(LogLevel level, string message){
            if (level >= LogLevel.Error){
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (level >= LogLevel.Warning){
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            Console.Write(message);
            Console.ResetColor();
        }
    }
}