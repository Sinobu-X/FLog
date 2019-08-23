# FLog
Fast Async Log for .NET Core

### Nuget
```json
Install-Package FLogCore
```


### JSON Config File
```json
{
  "FLog": {
    "async": true,
    "formatterString": "%date [%thread] %level %logger - %message%newline",
    "handlers": [
      {
        "type": "Console",
        "level": "Info",
        "includes": [
          "*"
        ],
        "excludes": [
        ]
      },
      {
        "type": "File",
        "level": "Error",
        "includes": [
          "*"
        ],
        "excludes": [
          "FlogConsoleAppTest.DBTest"
        ],
        "folder": "logs",
        "fileName": "[yyyyMMdd].main.log"
      },
      {
        "type": "File",
        "level": "Info",
        "formatterString": "%date - %message%newline",
        "includes": [
          "FlogConsoleAppTest.DBTest"
        ],
        "excludes": [
        ],
        "folder": "logs",
        "fileName": "[yyyyMMdd].db.log"
      }
    ]
  }
}
```

### How to use in Console Application
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


### How to use in Web Application
#### Config
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
#### Action
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

