using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FLog.Handlers;
using Newtonsoft.Json;

namespace FLog
{
    public class LogRepository
    {
        internal const string DEFAULT_NAME = "Default";

        public string Name{ get; }
        public LogFormatter Formatter{ get; set; }
        public string FormatterString{ get; set; }
        private readonly List<ILogHandler> _handlers = new List<ILogHandler>();

        public LogLevel Level{
            get{
                var minLevel = LogLevel.Critical;
                foreach (var handler in _handlers){
                    if (handler.Level < minLevel){
                        minLevel = handler.Level;
                    }
                }

                return minLevel;
            }
        }

        public LogRepository(string name){
            Name = name;
        }

        public LogRepository AddHandler(ILogHandler handler){
            _handlers.Add(handler);
            return this;
        }

        public LogRepository AddConsole(){
            var handler = new LogConsoleHandler();
            AddHandler(handler);
            return this;
        }

        public LogRepository AddConsole(Action<LogConsoleHandler> handlerDelegate){
            var handler = new LogConsoleHandler();
            handlerDelegate(handler);
            AddHandler(handler);
            return this;
        }

        public LogRepository AddDebug(){
            var handler = new LogDebugHandler();
            AddHandler(handler);
            return this;
        }

        public LogRepository AddDebug(Action<LogDebugHandler> handlerDelegate){
            var handler = new LogDebugHandler();
            handlerDelegate(handler);
            AddHandler(handler);
            return this;
        }

        public LogRepository AddFile(Action<LogFileHandler> handlerDelegate){
            var handler = new LogFileHandler();
            handlerDelegate(handler);
            AddHandler(handler);
            return this;
        }

        public LogRepository ClearHandlers(){
            _handlers.Clear();
            return this;
        }

        public Logger GetLogger(string loggerName){
            return new Logger(Name, loggerName);
        }

        public Logger GetLogger(Type type){
            return new Logger(Name, type.Name);
        }

        public async Task Write(List<LogData> items){
            foreach (var handler in _handlers){
                var subItems = items.FindAll(y => y.Level >= handler.Level);
                if (subItems.Count > 0){
                    await handler.Write(this, subItems);
                }
            }
        }
    }
}