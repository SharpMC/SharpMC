using SharpMC.API.Entities;

namespace SharpMC.Net
{
    public interface IClientWrapper
    {
        IPlayer Player { get; }
    }
}