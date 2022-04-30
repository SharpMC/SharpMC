using System;

namespace SharpMC.Plugin.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnPlayerJoinAttribute : Attribute
    {
    }
}