# FLog
Fast Async Log for .NET Core

##### JSON Config File
```json
{
  "FLog": {
    "async": true,
    "repositories": [
      {
        "name": "Default",
        "formatterString": "%date [%thread] %level %logger - %message%newline",
        "handlers": [
          {
            "type": "Console",
            "level": "Info"
          },
          {
            "type": "File",
            "level": "Error",
            "folder": "logs",
            "fileName": "[yyyyMMdd].log"
          }
        ]
      }
    ]
  }
}
```

##### How to use in Console Application
```csharp
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
    }
}
```


##### How to use in Web Application
###### Config
```csharp
public class Program
{
    public static void Main(string[] args){
        //Log Configure
        LogManager.Configure(Path.Combine(Directory.GetCurrentDirectory(), "flog.json"), true);

        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostBuilder, configureLogging) => {
                configureLogging.ClearProviders();
                configureLogging.AddFLog();
            })
            .UseStartup<Startup>();
}
```
###### Action
```csharp
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private static readonly Logger _logger = LogManager.GetLogger(typeof(ValuesController));

    // GET api/values
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get(){
        _logger.Info("Test Get");
        return new string[]{"value1", "value2"};
    }
}
```

