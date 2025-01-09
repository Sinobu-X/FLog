using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FLog;
using FLog.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FLogWebApplicationTest
{
    public class Program
    {
        public static void Main(string[] args){
            //Log Configure
            //LogManager.Configure(Path.Combine(Directory.GetCurrentDirectory(), "flog.json"), true);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostBuilder, configureLogging) => {
                    configureLogging.ClearProviders();
                    configureLogging.AddFLog((configuration) => {
                        configuration.JsonFilePath =Path.Combine(Directory.GetCurrentDirectory(), "flog.json");
                        configuration.JsonFileReloadOnChange = true;
                    });
                })
                .UseStartup<Startup>();
    }
}