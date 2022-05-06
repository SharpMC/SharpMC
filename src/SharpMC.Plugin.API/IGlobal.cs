using SharpMC.API.Entities;

namespace SharpMC.Plugin.API
{
    public interface IGlobal
    {
        void BroadcastChat(string message, IPlayer player);

        void StopServer(string message);
    }
}