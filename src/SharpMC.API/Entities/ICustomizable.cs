using SharpMC.API.Custom;

namespace SharpMC.API.Entities
{
    public interface ICustomizable
    {
        AttributeInstance GetAttribute(AttributeNames name);
    }
}