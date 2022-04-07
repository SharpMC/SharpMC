using System;

namespace SharpMC.Plugins.API
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class CommandAttribute : Attribute
	{
		public string Command;
		public string Description;
		public string Usage;
	}
}