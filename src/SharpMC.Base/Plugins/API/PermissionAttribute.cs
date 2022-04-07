using System;

namespace SharpMC.Plugins.API
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class PermissionAttribute : Attribute
	{
		public PermissionAttribute()
		{
			Permission = "";
		}

		public string Permission { get; set; }
	}
}