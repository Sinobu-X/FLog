using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using FLog.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FLog
{
    public static class LogManager
    {
        private static LogRepository _defaultRepository = new LogRepository(LogRepository.DEFAULT_NAME);
        private static List<LogRepository> _repositories = new List<LogRepository>(){_defaultRepository};
        public static LogFormatter Formatter{ get; set; }
        public static string FormatterString{ get; set; }
        public static bool Async = true;

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
                GetLogger(typeof(LogManager)).Info("FLog setting reload.");
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

            Async = rootNode.GetValue<bool>("async", true);
            
            var repositoriesNode = rootNode.GetSection("repositories");
            if (repositoriesNode == null){
                return;
            }
            
            ClearRepositories();

            foreach (var repositoryNode in repositoriesNode.GetChildren()){
                LogRepository repository;
                var name = repositoryNode.GetValue("name", LogRepository.DEFAULT_NAME);
                if (string.IsNullOrEmpty(name) ||
                    LogRepository.DEFAULT_NAME.Equals(name, StringComparison.OrdinalIgnoreCase)){
                    repository = _defaultRepository;
                }
                else{
                    repository = new LogRepository(name);
                    AddRepository(repository);
                }

                repository.FormatterString = repositoryNode.GetValue("formatterString", "");

                var handlersNode = repositoryNode.GetSection("handlers");
                if (handlersNode == null){
                    continue;
                }

                foreach (var handlerNode in handlersNode.GetChildren()){
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
                                                "Invalid type [" + typeName + "].");
                        }

                        if (!typeof(ILogHandler).IsAssignableFrom(type)){
                            throw new Exception("Wrong data in FLog json file.\n" +
                                                "Type [" + typeName + "] not extends from ILogHandler.");
                        }
                    }

                    var handler = Activator.CreateInstance(type) as ILogHandler;
                    if (handler == null){
                        if (!type.IsAssignableFrom(typeof(ILogHandler))){
                            throw new Exception("Wrong data in FLog json file.\n" +
                                                "Failed to create for type [" + typeName + "]");
                        }
                    }

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

                    foreach (var keyValuePair in handlerNode.AsEnumerable()){
                        var key = keyValuePair.Key;
                        var pos = key.LastIndexOf(":", StringComparison.OrdinalIgnoreCase);
                        if (pos >= 0){
                            key = key.Substring(pos + 1);
                        }
                        
                        if ("type".Equals(key, StringComparison.OrdinalIgnoreCase)){
                            continue;
                        }
                        else if ("Level".Equals(key, StringComparison.OrdinalIgnoreCase)){
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

                    repository.AddHandler(handler);
                }
            }
        }

        public static void AddRepository(LogRepository repository){
            _repositories.Add(repository);
        }

        public static LogRepository GetRepository(){
            return _defaultRepository;
        }

        public static LogRepository GetRepository(string name){
            return _repositories.Find(x => x.Name.Equals(name, StringComparison.Ordinal));
        }

        public static void ClearRepositories(){
            _repositories.Clear();
            _defaultRepository.ClearHandlers();
            _repositories.Add(_defaultRepository);
        }


        public static Logger GetLogger(Type type){
            return _defaultRepository.GetLogger(type);
        }

        public static Logger GetLogger(string loggerName){
            return _defaultRepository.GetLogger(loggerName);
        }

        public static Logger GetLogger(string repositoryName, Type type){
            return GetRepository(repositoryName)?.GetLogger(type);
        }

        public static Logger GetLogger(string repositoryName, string loggerName){
            return GetRepository(repositoryName)?.GetLogger(loggerName);
        }

        internal static async Task Write(List<LogData> items){
            foreach (var repository in _repositories){
                var subItems = items.FindAll(y => y.RepositoryName == repository.Name);
                if (subItems.Count > 0){
                    await repository.Write(subItems);
                }
            }
        }
    }
}