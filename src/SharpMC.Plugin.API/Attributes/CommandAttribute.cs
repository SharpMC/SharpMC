using System;

namespace SharpMC.Plugin.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        public string Command;
        public string Description;
        public string Usage;
    }
}