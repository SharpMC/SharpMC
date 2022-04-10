using System;

namespace SharpMC.API.Attributes
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