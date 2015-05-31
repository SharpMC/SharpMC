// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SharpMC.Entity;
using SharpMC.Utils;

namespace SharpMC.API
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
				foreach (var parameter in parameters)
				{
					sb.AppendFormat("<{0}> ", parameter.Name);
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
							player.SendChat("\u00A7cYou are not permitted to use this command!");
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
			player.SendChat("\u00A7cUnknown command.");
		}

		private bool ExecuteCommand(MethodInfo method, Player player, string[] args, CommandAttribute commandAttribute)
		{
			var parameters = method.GetParameters();

			var addLenght = 0;
			if (parameters.Length > 0 && parameters[0].ParameterType == typeof (Player))
			{
				addLenght = 1;
			}

			if (parameters.Length != args.Length + addLenght)
            {
                player.SendChat("Invalid parameters specified!");
                player.SendChat(commandAttribute.Usage);
                return true;
            }


			var objectArgs = new object[parameters.Length];

			for (var k = 0; k < parameters.Length; k++)
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

				return false;
			}

			var pluginInstance = _plugins.FirstOrDefault(plugin => plugin.GetType() == method.DeclaringType);
			if (pluginInstance == null) return false;

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