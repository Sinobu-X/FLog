using System;

namespace FLog
{
    public class LogData
    {
        public string LoggerName;
        public DateTime Time;
        public int ThreadId;
        public LogLevel Level;
        public string Message;
        public Exception Error;
        public bool ForceWrite;
    }
}