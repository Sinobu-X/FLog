using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLog.Handlers
{
    public class LogFileHandler : ILogHandler
    {
        public string Folder{ get; set; }
        public string FileName{ get; set; }
        public LogLevel Level{ get; set; } = LogLevel.Information;
        public LogFormatter Formatter{ get; set; }
        public string FormatterString{ get; set; }

        public async Task<bool> Write(LogRepository repository, List<LogData> items){
            var fileName = FileName;
            if (string.IsNullOrEmpty(fileName)){
                fileName = "[yyyyMMdd].log";
            }

            var pos1 = fileName.IndexOf("[", StringComparison.Ordinal);
            if (pos1 >= 0){
                var pos2 = fileName.IndexOf("]", pos1, StringComparison.Ordinal);
                if (pos2 > 0){
                    var pattern = fileName.Substring(pos1 + 1, pos2 - pos1 - 1);
                    try{
                        fileName = fileName.Substring(0, pos1) +
                                   items[0].Time.ToString(pattern) +
                                   fileName.Substring(pos2 + 1);
                    }
                    catch (Exception){
                        //nothing
                    }
                }
            }

            try{
                if (!Directory.Exists(Folder)){
                    Directory.CreateDirectory(Folder);
                }

                var sb = new StringBuilder();
                foreach (var item in items){
                    sb.Append(LogHelper.BuildLog(Formatter, FormatterString, repository, item));
                }

                using (var sw = new StreamWriter(Path.Combine(Folder, fileName), true, Encoding.UTF8)){
                    await sw.WriteAsync(sb.ToString());
                }

                return true;
            }
            catch (Exception){
                return false;
            }
        }
    }
}