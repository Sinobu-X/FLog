using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FLog.Handlers;
using Microsoft.Extensions.Configuration;


namespace FLog
{
    public static class LogManager
    {
        public static bool Async = true;
        public static LogFormatter Formatter{ get; set; }
        public static string FormatterString{ get; set; }
        public static readonly List<ILogHandler> Handlers = new List<ILogHandler>();

        private static IDisposable _registerChangeCallback;

        public static void Configure(string jsonFilePath, bool reloadOnChange){
            var builder = new ConfigurationBuilder().AddJsonFile(jsonFilePath, false, reloadOnChange);
            var configuration = builder.Build();
            LoadSetting(configuration);
            _registerChangeCallback =
                configuration.GetReloadToken().RegisterChangeCallback(OnSettingChanged, configuration);
        }

        private static void OnSettingChanged(object state){
            _registerChangeCallback?.Dispose();
            var configuration = (IConfigurationRoot) state;
            try{
                LoadSetting(configuration);
                GetLogger(typeof(LogManager)).Info("FLog setting reloaded successfully.");
            }
            catch (Exception ex){
                GetLogger(typeof(LogManager)).Error("", ex);
            }

            _registerChangeCallback = configuration.GetReloadToken().RegisterChangeCallback(OnSettingChanged, state);
        }

        private static void LoadSetting(IConfigurationRoot configuration){
            var rootNode = configuration.GetSection("FLog");
            if (rootNode == null){
                return;
            }

            //async
            Async = rootNode.GetValue("async", true);

            //formatterString
            FormatterString = rootNode.GetValue("formatterString", "");

            //handlers
            Handlers.Clear();
            var handlersNode = rootNode.GetSection("handlers");
            if (handlersNode != null){
                foreach (var handlerNode in handlersNode.GetChildren()){
                    //type
                    Type type;
                    var typeName = handlerNode.GetValue("type", "");
                    if (string.IsNullOrEmpty(typeName) ||
                        "Console".Equals(typeName, StringComparison.OrdinalIgnoreCase)){
                        type = typeof(LogConsoleHandler);
                    }
                    else if ("Debug".Equals(typeName, StringComparison.OrdinalIgnoreCase)){
                        type = typeof(LogDebugHandler);
                    }
                    else if ("File".Equals(typeName, StringComparison.OrdinalIgnoreCase)){
                        type = typeof(LogFileHandler);
                    }
                    else{
                        type = Type.GetType(typeName);
                        if (type == null){
                            throw new Exception("Wrong data in FLog json file.\n" +
                                                "Invalid handler type [" + typeName + "].");
                        }

                        if (!typeof(ILogHandler).IsAssignableFrom(type)){
                            throw new Exception("Wrong data in FLog json file.\n" +
                                                "Type [" + typeName + "] not extends from ILogHandler.");
                        }
                    }

                    var handler = Activator.CreateInstance(type) as ILogHandler;
                    if (handler == null){
                        throw new Exception("Wrong data in FLog json file.\n" +
                                            "Failed to create for type [" + typeName + "]");
                    }

                    //level
                    var levelString = handlerNode.GetValue("level", "");
                    if ("All".Equals(levelString, StringComparison.OrdinalIgnoreCase)){
                        handler.Level = LogLevel.None;
                    }
                    else if ("Trace".Equals(levelString, StringComparison.OrdinalIgnoreCase)){
                        handler.Level = LogLevel.Trace;
                    }
                    else if ("Debug".Equals(levelString, StringComparison.OrdinalIgnoreCase)){
                        handler.Level = LogLevel.Debug;
                    }
                    else if ("Info".Equals(levelString, StringComparison.OrdinalIgnoreCase)){
                        handler.Level = LogLevel.Information;
                    }
                    else if ("Warning".Equals(levelString, StringComparison.OrdinalIgnoreCase)){
                        handler.Level = LogLevel.Warning;
                    }
                    else if ("Error".Equals(levelString, StringComparison.OrdinalIgnoreCase)){
                        handler.Level = LogLevel.Error;
                    }
                    else if ("Critical".Equals(levelString, StringComparison.OrdinalIgnoreCase)){
                        handler.Level = LogLevel.Critical;
                    }
                    else{
                        throw new Exception("Wrong data in FLog json file.\n" +
                                            "Invalid level [" + levelString + "]");
                    }

                    //includes
                    handler.Includes = new List<string>();
                    var includesNode = handlerNode.GetSection("includes");
                    foreach (var node in includesNode.GetChildren()){
                        handler.Includes.Add(node.Value);
                    }

                    //excludes
                    handler.Excludes = new List<string>();
                    var excludesNode = handlerNode.GetSection("excludes");
                    foreach (var node in excludesNode.GetChildren()){
                        handler.Excludes.Add(node.Value);
                    }
                    
                    //others
                    foreach (var keyValuePair in handlerNode.AsEnumerable()){
                        var key = keyValuePair.Key;
                        var pos = key.LastIndexOf(":", StringComparison.OrdinalIgnoreCase);
                        if (pos >= 0){
                            key = key.Substring(pos + 1);
                        }

                        if ("type".Equals(key, StringComparison.OrdinalIgnoreCase)){
                            continue;
                        }
                        else if ("level".Equals(key, StringComparison.OrdinalIgnoreCase)){
                            continue;
                        }
                        else if ("includes".Equals(key, StringComparison.OrdinalIgnoreCase)){
                            continue;
                        }
                        else if ("excludes".Equals(key, StringComparison.OrdinalIgnoreCase)){
                            continue;
                        }

                        var p = type.GetProperties().FirstOrDefault(x =>
                            x.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                        if (p == null){
//                            throw new Exception("Wrong data in FLog json file.\n" +
//                                                "Invalid property [" + keyValuePair.Key + "]");
                            continue;
                        }

                        try{
                            p.SetValue(handler, handlerNode.GetValue(p.PropertyType, key));
                        }
                        catch (Exception ex){
                            throw new Exception("Wrong data in FLog json file.\n" +
                                                "Invalid property value [" + keyValuePair.Key + "]", ex);
                        }
                    }
                    
                    Handlers.Add(handler);
                }
            }
        }

        public static void AddConsole(){
            var handler = new LogConsoleHandler();
            Handlers.Add(handler);
        }

        public static void AddConsole(Action<LogConsoleHandler> handlerDelegate){
            var handler = new LogConsoleHandler();
            handlerDelegate(handler);
            Handlers.Add(handler);
        }

        public static void AddDebug(){
            var handler = new LogDebugHandler();
            Handlers.Add(handler);
        }

        public static void AddDebug(Action<LogDebugHandler> handlerDelegate){
            var handler = new LogDebugHandler();
            handlerDelegate(handler);
            Handlers.Add(handler);
        }

        public static void AddFile(Action<LogFileHandler> handlerDelegate){
            var handler = new LogFileHandler();
            handlerDelegate(handler);
            Handlers.Add(handler);
        }

        public static Logger GetLogger(string loggerName){
            return new Logger(loggerName);
        }

        public static Logger GetLogger(Type type){
            return new Logger(type.FullName);
        }

        internal static bool HasHandlers(LogData logData){
            return Handlers.Exists(x => x.BelongTo(logData));
        }

        internal static async Task Write(List<LogData> items){
            foreach (var handler in Handlers){
                var subItems = items.FindAll(x => handler.BelongTo(x));
                if (subItems.Count > 0){
                    await handler.Write(subItems);
                }
            }
        }
        
        private static bool BelongTo(this ILogHandler logHandler, LogData logData){
            if (logData.Level < logHandler.Level){
                return false;
            }

            if (logHandler.Includes.Exists(x => {
                if (x.Equals("*", StringComparison.OrdinalIgnoreCase)){
                    return true;
                }

                if (x.EndsWith(".*", StringComparison.OrdinalIgnoreCase)){
                    return logData.LoggerName.StartsWith(x.Substring(0, x.Length - 1),
                        StringComparison.OrdinalIgnoreCase);
                }

                return logData.LoggerName.Equals(x, StringComparison.OrdinalIgnoreCase);
            })){
                if (logHandler.Excludes.Exists(x => {
                    if (x.EndsWith(".*", StringComparison.OrdinalIgnoreCase)){
                        return logData.LoggerName.StartsWith(x.Substring(0, x.Length - 1),
                            StringComparison.OrdinalIgnoreCase);
                    }

                    return logData.LoggerName.Equals(x, StringComparison.OrdinalIgnoreCase);
                })){
                    return false;
                }
                else{
                    return true;
                }
            }
            else{
                return false;
            }
        }
    }
}