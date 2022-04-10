using System;

namespace SharpMC.API.Attributes
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