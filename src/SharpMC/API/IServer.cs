using SharpMC.API.Players;

namespace SharpMC.API
{
    public interface IServer
    {
        void Start();

        void Stop();

        IEncryption RsaEncryption { get; }

        IPlayerFactory PlayerFactory { get; }

        IServerInfo Info { get; }
    }
}