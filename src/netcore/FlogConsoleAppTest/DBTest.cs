using System;
using FLog;

namespace FlogConsoleAppTest
{
    public class DBTest
    {
        private static Logger _log = LogManager.GetLogger(typeof(DBTest));

        public void TestInfo(){
            _log.Info("INSERT INTO TABLE() VALUES()");
        }
        
        public void TestWarning(){
            _log.Warning("INSERT INTO TABLE() VALUES()");
        }
        
        public void TestError(){
            _log.Error("Error",new Exception("Error"));
        }
    }
}