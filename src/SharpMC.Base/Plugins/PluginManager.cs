using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SharpMC.Core;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.Plugins.API;
using SharpMC.World;

namespace SharpMC.Plugins
{
	public class PluginManager
	{
		private readonly Dictionary<MethodInfo, CommandAttribute> _pluginCommands =
			new Dictionary<MethodInfo, CommandAttribute>();

		private readonly Dictionary<MethodInfo, OnPlayerJoinAttribute> _pluginPlayerJoinEvents =
			new Dictionary<MethodInfo, OnPlayerJoinAttribute>();

		private readonly List<object> _plugins = new List<object>();

		public List<object> Plugins
		{
			get { return _plugins; }
		}

		internal void LoadPlugins()
		{
			if (Config.GetProperty("PluginDisabled", false)) return;
			var pluginDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
			pluginDirectory = Config.GetProperty("PluginDirectory", pluginDirectory);
			if (pluginDirectory != null)
			{
				pluginDirectory = Path.GetFullPath(pluginDirectory);

				foreach (var pluginPath in Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories))
				{
					if (pluginPath.Contains("/ref/") || pluginPath.Contains("\\ref\\")) {
						// Skip modern reference assemblies
						continue;
					}

					var newAssembly = Assembly.LoadFile(pluginPath);
					var types = newAssembly.GetExportedTypes();
					foreach (var type in types)
					{
						try
						{
							if (!type.IsDefined(typeof (PluginAttribute), true) && !typeof (IPlugin).IsAssignableFrom(type)) continue;
							if (type.IsDefined(typeof (PluginAttribute), true))
							{
								var pluginAttribute = Attribute.GetCustomAttribute(type, typeof (PluginAttribute), true) as PluginAttribute;
								if (pluginAttribute != null)
								{
									if (!Config.GetProperty(pluginAttribute.PluginName + ".Enabled", true)) continue;
								}
							}
							var ctor = type.GetConstructor(Type.EmptyTypes);
							if (ctor != null)
							{
								var plugin = ctor.Invoke(null);
								_plugins.Add(plugin);
								LoadCommands(type);
								LoadOnPlayerJoin(type);
							}
						}
						catch (Exception ex)
						{
							ConsoleFunctions.WriteWarningLine("Failed loading plugin type " + type + " as a plugin.");
							ConsoleFunctions.WriteDebugLine("Plugin loader caught exception: " + ex);
						}
					}
				}
			}
		}

		private void LoadCommands(Type type)
		{
			var methods = type.GetMethods();
			foreach (var method in methods)
			{
				var commandAttribute = Attribute.GetCustomAttribute(method, typeof (CommandAttribute), false) as CommandAttribute;
				if (commandAttribute == null) continue;

				if (string.IsNullOrEmpty(commandAttribute.Command))
				{
					commandAttribute.Command = method.Name;
				}

				var sb = new StringBuilder();
				sb.Append("/");
				sb.Append(commandAttribute.Command);
				var parameters = method.GetParameters();
				if (parameters.Length > 0) sb.Append(" ");
				for (int i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i];
					if (i == 0 && parameter.ParameterType == typeof (Player)) continue; //source player
					
					sb.AppendFormat(parameter.IsOptional ? "[{0}] " : "<{0}> ", parameter.Name);
				}
				commandAttribute.Usage = sb.ToString().Trim();

				var descriptionAttribute =
					Attribute.GetCustomAttribute(method, typeof (DescriptionAttribute), false) as DescriptionAttribute;
				if (descriptionAttribute != null) commandAttribute.Description = descriptionAttribute.Description;

				_pluginCommands.Add(method, commandAttribute);
			}
		}

		private void LoadOnPlayerJoin(Type type)
		{
			var methods = type.GetMethods();
			foreach (var method in methods)
			{
				var commandAttribute =
					Attribute.GetCustomAttribute(method, typeof (OnPlayerJoinAttribute), false) as OnPlayerJoinAttribute;
				if (commandAttribute == null) continue;

				_pluginPlayerJoinEvents.Add(method, commandAttribute);
			}
		}

		internal void EnablePlugins(LevelManager levelman)
		{
			foreach (var plugin in _plugins)
			{
				var enablingPlugin = plugin as IPlugin;
				if (enablingPlugin == null) continue;

				try
				{
					enablingPlugin.OnEnable(new PluginContext(this));
				}
				catch (Exception ex)
				{
					ConsoleFunctions.WriteWarningLine("On enable plugin: " + ex);
				}
			}
		}

		internal void DisablePlugins()
		{
			foreach (var plugin in _plugins)
			{
				var enablingPlugin = plugin as IPlugin;
				if (enablingPlugin == null) continue;

				try
				{
					enablingPlugin.OnDisable();
				}
				catch (Exception ex)
				{
					ConsoleFunctions.WriteWarningLine("On disable plugin: " + ex);
				}
			}
		}

		public void HandleCommand(string message, Player player)
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
					if (!commandText.Equals(commandAttribute.Command, StringComparison.InvariantCultureIgnoreCase)) continue;

					var method = handlerEntry.Key;
					if (method == null) return;

					var authorizationAttributes = method.GetCustomAttributes<PermissionAttribute>(true);
					foreach (var authorizationAttribute in authorizationAttributes)
					{
						if (!PermissionManager.HasPermission(player, authorizationAttribute.Permission))
						{
							player.SendChat("You are not permitted to use this command!", ChatColor.Red);
							return;
						}
					}
					if (ExecuteCommand(method, player, arguments, commandAttribute)) return;
				}
			}
			catch (Exception ex)
			{
				ConsoleFunctions.WriteWarningLine(ex.ToString());
			}
			player.SendChat("Unknown command.", ChatColor.Red);
		}

		private bool ExecuteCommand(MethodInfo method, Player player, string[] args, CommandAttribute commandAttribute)
		{
			var parameters = method.GetParameters();

			var addLenght = 0;
			int requiredParameters = 0;
			if (parameters.Length > 0 && parameters[0].ParameterType == typeof (Player))
			{
				addLenght = 1;
				requiredParameters = -1;
			}

			bool hasRequiredParameters = true;
			bool hasStringArray = false;

			foreach (var param in parameters)
			{
				if (!param.IsOptional) requiredParameters++;
				if (param.ParameterType == typeof (string[])) hasStringArray = true;
			}

			if (args.Length < requiredParameters && !hasStringArray) hasRequiredParameters = false;

			if (!hasRequiredParameters || args.Length > (parameters.Length - addLenght) && !hasStringArray)
			{
				player.SendChat("Invalid command usage!", ChatColor.Red);
				player.SendChat(commandAttribute.Usage, ChatColor.Red);
				return true;
			}

			var objectArgs = new object[parameters.Length];

			bool stringarrayfound = false;
			int stringarrayposition = 0;
			List<string> stringarrayvalues = new List<string>();

			int length = args.Length + addLenght;
			for (var k = 0; k < length; k++)
			{
				var parameter = parameters[k];
				var i = k - addLenght;
				if (k == 0 && addLenght == 1)
				{
					if (parameter.ParameterType == typeof (Player))
					{
						objectArgs[k] = player;
						continue;
					}
					ConsoleFunctions.WriteWarningLine("Command method " + method.Name + " missing Player as first argument.");
					return false;
				}

				if (parameter.ParameterType == typeof (string[]))
				{
					stringarrayfound = true;
					stringarrayposition = k;
					stringarrayvalues.Add(args[i]);

					objectArgs[stringarrayposition] = stringarrayvalues.ToArray();
					break;
				}

				if (parameter.ParameterType == typeof (string))
				{
					objectArgs[k] = args[i];
					continue;
				}

				if (parameter.ParameterType == typeof (byte))
				{
					byte value;
					if (!byte.TryParse(args[i], out value)) return false;
					objectArgs[k] = value;
					continue;
				}

				if (parameter.ParameterType == typeof (short))
				{
					short value;
					if (!short.TryParse(args[i], out value)) return false;
					objectArgs[k] = value;
					continue;
				}

				if (parameter.ParameterType == typeof (int))
				{
					int value;
					if (!int.TryParse(args[i], out value)) return false;
					objectArgs[k] = value;
					continue;
				}

				if (parameter.ParameterType == typeof (bool))
				{
					bool value;
					if (!bool.TryParse(args[i], out value)) return false;
					objectArgs[k] = value;
					continue;
				}

				if (parameter.ParameterType == typeof (float))
				{
					float value;
					if (!float.TryParse(args[i], out value)) return false;
					objectArgs[k] = value;
					continue;
				}

				if (parameter.ParameterType == typeof (double))
				{
					double value;
					if (!double.TryParse(args[i], out value)) return false;
					objectArgs[k] = value;
					continue;
				}

				if (parameter.ParameterType == typeof(Player))
				{
					Player value = Globals.LevelManager.GetAllPlayers().FirstOrDefault(p => p.Username.ToLower().Equals(args[i].ToLower()));
					if (value == null)
					{
						player.SendChat(String.Format("Player \"{0}\" is not found!", args[i]), ChatColor.Red);
						return true;
					}
					objectArgs[k] = value;
                    continue;
				}

				return false;
			}

			if (stringarrayfound)
			{
				for (int k = stringarrayposition + 1; k <= args.Length; k++)
				{
					var i = k - addLenght;
					stringarrayvalues.Add(args[i]);
					objectArgs[stringarrayposition] = stringarrayvalues.ToArray();
				}
			}

			var pluginInstance = _plugins.FirstOrDefault(plugin => plugin.GetType() == method.DeclaringType);
			if (pluginInstance == null)
			{
				ConsoleFunctions.WriteDebugLine("Plugin instance is null!");
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
					var atrib = handler.Value;

					var method = handler.Key;
					if (method == null) continue;
					if (method.IsStatic)
					{
						method.Invoke(null, new object[] {player});
					}
					else
					{
						var pluginInstance = _plugins.FirstOrDefault(plugin => plugin.GetType() == method.DeclaringType);
						if (pluginInstance == null) continue;

						if (method.ReturnType == typeof (void))
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
				//For now we will just ignore this, not to big of a deal.
				//Will have to think a bit more about this later on.
				ConsoleFunctions.WriteWarningLine("Plugin error: " + ex);
			}
		}
	}
}