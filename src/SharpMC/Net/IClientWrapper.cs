using SharpMC.Players;

namespace SharpMC.Net
{
    public interface IClientWrapper
    {
        Player Player { get; }
    }
}