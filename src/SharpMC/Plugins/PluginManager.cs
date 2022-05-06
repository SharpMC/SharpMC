using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using SharpMC.API;
using SharpMC.API.Worlds;
using SharpMC.Config;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.Plugin.API;
using SharpMC.Plugin.API.Attributes;
using static SharpMC.Util.PathTool;

namespace SharpMC.Plugins
{
    internal sealed class PluginManager : IPluginManager, IDisposable
    {
        private readonly ILogger<PluginManager> _log;
        private readonly IHostEnv _host;
        private readonly IOptions<ServerSettings> _config;
        private readonly ILevelManager _levelManager;
        private readonly IPermissionManager _permissionManager;
        private readonly ILoggerFactory _factory;

        private readonly List<IPlugin> _plugins;
        private readonly Dictionary<MethodInfo, CommandAttribute> _commands;
        private readonly Dictionary<MethodInfo, OnPlayerJoinAttribute> _joinEvents;

        public PluginManager(IHostEnv host, ILogger<PluginManager> log, ILevelManager levelMgr,
            IOptions<ServerSettings> config, IEnumerable<IPlugin> builtIns, 
            IPermissionManager permManager, ILoggerFactory factory)
        {
            _commands = new Dictionary<MethodInfo, CommandAttribute>();
            _joinEvents = new Dictionary<MethodInfo, OnPlayerJoinAttribute>();

            _factory = factory;
            _log = log;
            _config = config;
            _permissionManager = permManager;
            _levelManager = levelMgr;
            _host = host;
            _plugins = new List<IPlugin>();
            Array.ForEach(builtIns.ToArray(), LoadPlugin);
        }

        private string PluginDirectory
            => Ensure(_host, _config.Value.Plugins?.Directory ?? nameof(Plugins));

        public void EnablePlugins(ILevelManager levelManager)
        {
            foreach (var plugin in _plugins)
            {
                if (plugin is not { } one)
                    continue;
                try
                {
                    var context = new PluginContext(levelManager, default, _factory);
                    one.OnEnable(context);
                }
                catch (Exception ex)
                {
                    _log.LogWarning(ex, "On enable plugin!");
                }
            }
        }

        public void DisablePlugins()
        {
            foreach (var plugin in _plugins)
            {
                if (plugin is not { } one)
                    continue;
                try
                {
                    one.OnDisable();
                }
                catch (Exception ex)
                {
                    _log.LogWarning(ex, "On disable plugin!");
                }
            }
        }

        public void LoadPlugins()
        {
            if (_config.Value.Plugins?.Disabled ?? false)
                return;
            var dir = PluginDirectory;
            const SearchOption o = SearchOption.AllDirectories;
            var files = Directory.GetFiles(dir, "*.dll", o);
            foreach (var pluginPath in files)
                try
                {
                    LoadPlugin(pluginPath);
                }
                catch (Exception ex)
                {
                    _log.LogWarning(ex, pluginPath);
                }
        }

        private void LoadPlugin(string pluginPath)
        {
            var newAssembly = Assembly.LoadFile(pluginPath);
            var types = newAssembly.GetExportedTypes();
            foreach (var type in types)
            {
                LoadPlugin(type, null);
            }
        }

        private void LoadPlugin(IPlugin plugin)
        {
            LoadPlugin(plugin.GetType(), plugin);
        }

        private void LoadPlugin(Type type, object? instance)
        {
            try
            {
                if (!type.IsDefined(typeof(PluginAttribute), true) &&
                    !typeof(IPlugin).IsAssignableFrom(type))
                    return;
                if (type.IsDefined(typeof(PluginAttribute), true))
                {
                    if (Attribute.GetCustomAttribute(type, typeof(PluginAttribute), true)
                        is PluginAttribute pluginAttribute)
                    {
                        var key = $"{pluginAttribute.PluginName}.Enabled";
                        // TODO Add specific disable switch
                        if (_config.Value.Plugins?.Disabled ?? false)
                            return;
                    }
                }
                var plugin = instance
                             ?? type.GetConstructor(Type.EmptyTypes)?.Invoke(null);
                _plugins.Add((IPlugin) plugin!);
                LoadCommands(type);
                LoadOnPlayerJoin(type);
            }
            catch (Exception ex)
            {
                _log.LogWarning($"Failed loading plugin type {type} as a plugin.");
                _log.LogDebug(ex, "Plugin loader caught exception!");
            }
        }

        private void LoadOnPlayerJoin(Type type)
        {
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (!(Attribute.GetCustomAttribute(method, typeof(OnPlayerJoinAttribute), false)
                        is OnPlayerJoinAttribute commandAttribute))
                    continue;
                _joinEvents.Add(method, commandAttribute);
            }
        }

        private void LoadCommands(Type type)
        {
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (!(Attribute.GetCustomAttribute(method, typeof(CommandAttribute), false)
                        is CommandAttribute commandAttribute))
                    continue;
                if (string.IsNullOrEmpty(commandAttribute.Command))
                {
                    commandAttribute.Command = method.Name;
                }
                var sb = new StringBuilder();
                sb.Append("/");
                sb.Append(commandAttribute.Command);
                var parameters = method.GetParameters();
                if (parameters.Length > 0) sb.Append(" ");
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    if (i == 0 && parameter.ParameterType == typeof(IPlayer))
                        continue;
                    sb.AppendFormat(parameter.IsOptional ? "[{0}] " : "<{0}> ", parameter.Name);
                }
                commandAttribute.Usage = sb.ToString().Trim();
                if (Attribute.GetCustomAttribute(method, typeof(DescriptionAttribute), false)
                    is DescriptionAttribute descriptionAttribute)
                    commandAttribute.Description = descriptionAttribute.Description;
                _commands.Add(method, commandAttribute);
            }
        }

        public void Dispose()
        {
            _joinEvents.Clear();
            _commands.Clear();
            _plugins.Clear();
        }

        #region Execute

        public void HandleCommand(string message, IPlayer player)
        {
            try
            {
                var commandText = message.Split(' ')[0];
                message = message.Replace(commandText, "").Trim();
                commandText = commandText.Replace("/", "").Replace(".", "");
                var arguments = message.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var handlerEntry in _commands)
                {
                    var commandAttribute = handlerEntry.Value;
                    if (!commandText.Equals(commandAttribute.Command, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    var method = handlerEntry.Key;
                    var authAttrs = method.GetCustomAttributes<PermissionAttribute>(true);
                    foreach (var authorizationAttribute in authAttrs)
                    {
                        var permissionManager = _permissionManager;
                        if (!permissionManager.HasPermission(player, authorizationAttribute.Permission))
                        {
                            player.SendChat("You are not permitted to use this command!", ChatColor.Red);
                            return;
                        }
                    }
                    if (ExecuteCommand(method, player, arguments, commandAttribute))
                        return;
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex.ToString());
            }
            player.SendChat("Unknown command.", ChatColor.Red);
        }

        private bool ExecuteCommand(MethodInfo method, IPlayer player, string[] args, CommandAttribute commandAttribute)
        {
            var parameters = method.GetParameters();
            var addLength = 0;
            var requiredParameters = 0;
            if (parameters.Length > 0 && parameters[0].ParameterType == typeof(IPlayer))
            {
                addLength = 1;
                requiredParameters = -1;
            }
            var hasRequiredParameters = true;
            var hasStringArray = false;
            foreach (var param in parameters)
            {
                if (!param.IsOptional) requiredParameters++;
                if (param.ParameterType == typeof(string[])) hasStringArray = true;
            }
            if (args.Length < requiredParameters && !hasStringArray)
                hasRequiredParameters = false;
            if (!hasRequiredParameters || args.Length > parameters.Length - addLength && !hasStringArray)
            {
                player.SendChat("Invalid command usage!", ChatColor.Red);
                player.SendChat(commandAttribute.Usage, ChatColor.Red);
                return true;
            }
            var objectArgs = new object[parameters.Length];
            var stringArrayFound = false;
            var stringArrayPosition = 0;
            var stringArrayValues = new List<string>();
            var length = args.Length + addLength;
            for (var k = 0; k < length; k++)
            {
                var parameter = parameters[k];
                var i = k - addLength;
                if (k == 0 && addLength == 1)
                {
                    if (parameter.ParameterType == typeof(IPlayer))
                    {
                        objectArgs[k] = player;
                        continue;
                    }
                    _log.LogWarning($"Command method {method.Name} missing player as first argument.");
                    return false;
                }
                if (parameter.ParameterType == typeof(string[]))
                {
                    stringArrayFound = true;
                    stringArrayPosition = k;
                    stringArrayValues.Add(args[i]);
                    objectArgs[stringArrayPosition] = stringArrayValues.ToArray();
                    break;
                }
                if (parameter.ParameterType == typeof(string))
                {
                    objectArgs[k] = args[i];
                    continue;
                }
                if (parameter.ParameterType == typeof(byte))
                {
                    if (!byte.TryParse(args[i], out var value))
                        return false;
                    objectArgs[k] = value;
                    continue;
                }
                if (parameter.ParameterType == typeof(short))
                {
                    if (!short.TryParse(args[i], out var value))
                        return false;
                    objectArgs[k] = value;
                    continue;
                }
                if (parameter.ParameterType == typeof(int))
                {
                    if (!int.TryParse(args[i], out var value))
                        return false;
                    objectArgs[k] = value;
                    continue;
                }
                if (parameter.ParameterType == typeof(bool))
                {
                    if (!bool.TryParse(args[i], out var value))
                        return false;
                    objectArgs[k] = value;
                    continue;
                }
                if (parameter.ParameterType == typeof(float))
                {
                    if (!float.TryParse(args[i], out var value))
                        return false;
                    objectArgs[k] = value;
                    continue;
                }
                if (parameter.ParameterType == typeof(double))
                {
                    if (!double.TryParse(args[i], out var value))
                        return false;
                    objectArgs[k] = value;
                    continue;
                }
                if (parameter.ParameterType == typeof(IPlayer))
                {
                    var value = _levelManager.GetAllPlayers()
                        .FirstOrDefault(p => p.UserName.ToLower().Equals(args[i].ToLower()));
                    if (value == null)
                    {
                        player.SendChat($"Player \"{args[i]}\" is not found!", ChatColor.Red);
                        return true;
                    }
                    objectArgs[k] = value;
                    continue;
                }
                return false;
            }
            if (stringArrayFound)
            {
                for (var k = stringArrayPosition + 1; k <= args.Length; k++)
                {
                    var i = k - addLength;
                    stringArrayValues.Add(args[i]);
                    objectArgs[stringArrayPosition] = stringArrayValues.ToArray();
                }
            }
            var pluginInstance = _plugins.FirstOrDefault(p => p.GetType() == method.DeclaringType);
            if (pluginInstance == null)
            {
                _log.LogDebug("Plugin instance is null!");
                return false;
            }
            if (method.IsStatic)
            {
                method.Invoke(null, objectArgs);
            }
            else
            {
                if (method.DeclaringType == null)
                    return false;
                method.Invoke(pluginInstance, objectArgs);
            }
            return true;
        }

        internal void HandlePlayerJoin(IPlayer player)
        {
            try
            {
                foreach (var handler in _joinEvents)
                {
                    var method = handler.Key;
                    if (method.IsStatic)
                    {
                        method.Invoke(null, new object[] {player});
                    }
                    else
                    {
                        var pluginInstance = _plugins.FirstOrDefault(p => p.GetType() == method.DeclaringType);
                        if (pluginInstance == null)
                            continue;
                        if (method.ReturnType == typeof(void))
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length == 1)
                            {
                                method.Invoke(pluginInstance, new object[] {player});
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Plugin error!");
            }
        }

        #endregion
    }
}