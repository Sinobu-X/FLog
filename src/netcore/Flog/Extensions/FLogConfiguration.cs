using System;

namespace FLog.Extensions
{
    public class FLogConfiguration
    {
        public string JsonFilePath{ get; set; }
        public bool JsonFileReloadOnChange{ get; set; }
        public Func<Exception, bool> ExceptionFilter{ get; set; }
    }
}