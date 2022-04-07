using System;

namespace SharpMC.Plugins.API
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PluginAttribute : Attribute
	{
		public string Author;
		public string Description;
		public string PluginName;
		public string PluginVersion;
	}
}