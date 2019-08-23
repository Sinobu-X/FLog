using FLog;

namespace FlogConsoleAppTest
{
    public class DBTest
    {
        private static Logger _log = LogManager.GetLogger(typeof(DBTest));

        public void Test(){
            _log.Info("INSERT INTO TABLE() VALUES()");
        }
    }
}