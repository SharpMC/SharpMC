using System;

namespace SharpMC.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnPlayerJoinAttribute : Attribute
    {
    }
}