using SharpMC.API.Entities;
using SharpMC.API.Net;

namespace SharpMC.API.Players
{
    public interface IPlayerFactory
    {
        IPlayer CreatePlayer(INetConnection connection, string username);
    }
}