using SharpMC.Net;

namespace SharpMC.Players
{
    public interface IPlayerFactory
    {
        Player CreatePlayer(McNetConnection connection, string username);
    }
}