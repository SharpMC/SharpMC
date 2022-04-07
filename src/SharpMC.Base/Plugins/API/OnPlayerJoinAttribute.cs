using System;

namespace SharpMC.Plugins.API
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class OnPlayerJoinAttribute : Attribute
	{
	}
}