using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using FLog;

namespace FlogConsoleAppTest
{
    class Program
    {
        private static Logger _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args){
            //Log Configure
            LogManager.Configure(Path.Combine(Directory.GetCurrentDirectory(), "flog.json"), true);

            //test
            _log.Info("Hello World!");
            _log.Warning("Warning Test");
            _log.Error("Error Test");
            

            new DBTest().TestInfo();
   

            while (true){
                _log.Info("Press Y to test speed.");
                var command = Console.ReadLine();
                if ("Y".Equals(command, StringComparison.OrdinalIgnoreCase)){
                    TestSpeed();
                }
                else{
                    break;
                }
            }
        }

        static void TestSpeed(){
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _log.Info("Test Start");
            for (var i = 0; i < 10000; i++){
                _log.Error($"THIS IS AN ERROR {i}");
            }

            _log.Info("Test End");
            sw.Stop();
            _log.Info($"Total: {sw.ElapsedMilliseconds}ms");
        }
    }
}