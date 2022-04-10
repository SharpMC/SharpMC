using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using SharpMC.API.Attributes;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.API.Plugins;
using SharpMC.API.Worlds;
using SharpMC.Logging;
using SharpMC.Players;

namespace SharpMC.Plugins
{
    public class PluginManager : IPluginManager
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(PluginManager));

        private readonly Dictionary<MethodInfo, CommandAttribute> _pluginCommands
            = new Dictionary<MethodInfo, CommandAttribute>();

        private readonly Dictionary<MethodInfo, OnPlayerJoinAttribute> _pluginPlayerJoinEvents
            = new Dictionary<MethodInfo, OnPlayerJoinAttribute>();

        private readonly List<IPlugin> _plugins = new List<IPlugin>();

        public List<IPlugin> Plugins => _plugins;

        public void LoadPlugins()
        {
            if (Config.GetProperty("PluginDisabled", false))
                return;
            var pluginDirectory = GetPluginDirectory();
            if (pluginDirectory == null)
                return;
            pluginDirectory = Path.GetFullPath(pluginDirectory);
            var files = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);
            foreach (var pluginPath in files)
                try
                {
                    LoadPlugin(pluginPath);
                }
                catch (Exception)
                {
                    // Ignore that!
                }
        }

        private void LoadPlugin(string pluginPath)
        {
            var newAssembly = Assembly.LoadFile(pluginPath);
            var types = newAssembly.GetExportedTypes();
            foreach (var type in types)
            {
                try
                {
                    if (!type.IsDefined(typeof(PluginAttribute), true) &&
                        !typeof(IPlugin).IsAssignableFrom(type))
                        continue;
                    if (type.IsDefined(typeof(PluginAttribute), true))
                    {
                        if (Attribute.GetCustomAttribute(type, typeof(PluginAttribute), true)
                            is PluginAttribute pluginAttribute)
                        {
                            var key = $"{pluginAttribute.PluginName}.Enabled";
                            if (!Config.GetProperty(key, true))
                                continue;
                        }
                    }
                    var ctor = type.GetConstructor(Type.EmptyTypes);
                    if (ctor != null)
                    {
                        var plugin = ctor.Invoke(null);
                        _plugins.Add((IPlugin) plugin);
                        LoadCommands(type);
                        LoadOnPlayerJoin(type);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Failed loading plugin type {type} as a plugin.");
                    Log.LogDebug(ex, "Plugin loader caught exception!");
                }
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
                _pluginCommands.Add(method, commandAttribute);
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
                _pluginPlayerJoinEvents.Add(method, commandAttribute);
            }
        }

        public void EnablePlugins(ILevelManager manager)
        {
            foreach (var plugin in _plugins)
            {
                if (!(plugin is IPlugin enablingPlugin))
                    continue;
                try
                {
                    enablingPlugin.OnEnable(new PluginContext(this));
                }
                catch (Exception ex)
                {
                    Log.LogWarning(ex, "On enable plugin!");
                }
            }
        }

        public void DisablePlugins()
        {
            foreach (var plugin in _plugins)
            {
                if (!(plugin is IPlugin enablingPlugin))
                    continue;
                try
                {
                    enablingPlugin.OnDisable();
                }
                catch (Exception ex)
                {
                    Log.LogWarning(ex, "On disable plugin!");
                }
            }
        }

        public void HandleCommand(string message, IPlayer player)
        {
            try
            {
                var commandText = message.Split(' ')[0];
                message = message.Replace(commandText, "").Trim();
                commandText = commandText.Replace("/", "").Replace(".", "");
                var arguments = message.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var handlerEntry in _pluginCommands)
                {
                    var commandAttribute = handlerEntry.Value;
                    if (!commandText.Equals(commandAttribute.Command, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    var method = handlerEntry.Key;
                    if (method == null)
                        return;
                    var authAttrs = method.GetCustomAttributes<PermissionAttribute>(true);
                    foreach (var authorizationAttribute in authAttrs)
                    {
                        var permissionManager = Globals.Instance.PermissionManager;
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
                Log.LogWarning(ex.ToString());
            }
            player.SendChat("Unknown command.", ChatColor.Red);
        }

        private bool ExecuteCommand(MethodInfo method, IPlayer player, string[] args, CommandAttribute commandAttribute)
        {
            var parameters = method.GetParameters();
            var addLength = 0;
            var requiredParameters = 0;
            if (parameters.Length > 0 && parameters[0].ParameterType == typeof(Player))
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
            if (!hasRequiredParameters || args.Length > (parameters.Length - addLength) && !hasStringArray)
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
                    Log.LogWarning($"Command method {method.Name} missing player as first argument.");
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
                if (parameter.ParameterType == typeof(Player))
                {
                    var value = Globals.Instance.LevelManager.GetAllPlayers()
                        .FirstOrDefault(p => p.Username.ToLower().Equals(args[i].ToLower()));
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
            var pluginInstance = _plugins.FirstOrDefault(plugin => plugin.GetType() == method.DeclaringType);
            if (pluginInstance == null)
            {
                Log.LogDebug("Plugin instance is null!");
                return false;
            }
            if (method.IsStatic)
            {
                method.Invoke(null, objectArgs);
            }
            else
            {
                if (method.DeclaringType == null) return false;

                method.Invoke(pluginInstance, objectArgs);
            }
            return true;
        }

        internal void HandlePlayerJoin(Player player)
        {
            try
            {
                foreach (var handler in _pluginPlayerJoinEvents)
                {
                    var attrib = handler.Value;
                    var method = handler.Key;
                    if (method == null) continue;
                    if (method.IsStatic)
                    {
                        method.Invoke(null, new object[] {player});
                    }
                    else
                    {
                        var pluginInstance =
                            _plugins.FirstOrDefault(plugin => plugin.GetType() == method.DeclaringType);
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
                Log.LogWarning(ex, "Plugin error!");
            }
        }

        private static string GetPluginDirectory()
        {
            var assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var pluginDirectory = Path.GetDirectoryName(assemblyPath);
            pluginDirectory = Config.GetProperty("PluginDirectory", pluginDirectory);
            return pluginDirectory;
        }
    }
}