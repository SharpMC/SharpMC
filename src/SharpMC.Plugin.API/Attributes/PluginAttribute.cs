using System;

namespace SharpMC.Plugin.API.Attributes
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