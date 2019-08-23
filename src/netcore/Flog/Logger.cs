using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace FLog
{
    public class Logger
    {
        public string Name{ get; }

        private static readonly List<LogData> _logs = new List<LogData>();
        private static readonly object _locker = new object();
        private static bool _taskIsRunning = false;

        public Logger(string name){
            Name = name;
        }

        public void Debug(string message, Exception ex = null){
            Write(new LogData(){
                LoggerName = Name,
                Level = LogLevel.Debug,
                Message = message,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Time = DateTime.Now,
                Error = ex
            });
        }

        public void Info(string message, Exception ex = null){
            Write(new LogData(){
                LoggerName = Name,
                Level = LogLevel.Information,
                Message = message,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Time = DateTime.Now,
                Error = ex
            });
        }

        public void Warning(string message, Exception ex = null){
            Write(new LogData(){
                LoggerName = Name,
                Level = LogLevel.Warning,
                Message = message,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Time = DateTime.Now,
                Error = ex
            });
        }

        public void Error(string message, Exception ex = null){
            Write(new LogData(){
                LoggerName = Name,
                Level = LogLevel.Error,
                Message = message,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Time = DateTime.Now,
                Error = ex
            });
        }

        public void Critical(string message, Exception ex = null){
            Write(new LogData(){
                LoggerName = Name,
                Level = LogLevel.Critical,
                Message = message,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Time = DateTime.Now,
                Error = ex
            });
        }

        private static void Write(LogData logData){
            if (!LogManager.HasHandlers(logData)){
                return;
            }

            bool needCreateTask;

            lock (_locker){
                _logs.Add(logData);
                if (!_taskIsRunning){
                    needCreateTask = true;
                    _taskIsRunning = true;
                }
                else{
                    needCreateTask = false;
                }
            }

            if (needCreateTask){
                if (LogManager.Async){
                    Task.Run(async () => { await WriteAsync(); });
                }
                else{
                    Task.Run(async () => { await WriteAsync(); }).Wait();
                }
            }
        }

        private static async Task WriteAsync(){
            while (true){
                List<LogData> items;
                lock (_locker){
                    if (_logs.Count > 0){
                        items = new List<LogData>(_logs);
                        _logs.Clear();
                    }
                    else{
                        items = null;
                        _taskIsRunning = false;
                    }
                }

                if (items != null){
                    await LogManager.Write(items);
                }
                else{
                    break;
                }
            }
        }
    }
}