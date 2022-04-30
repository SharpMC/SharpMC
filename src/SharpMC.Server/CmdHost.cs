using SharpMC.API;

namespace SharpMC.Server
{
    internal sealed class CmdHost : IHostEnv
    {
        public CmdHost(string contentRoot)
        {
            ContentRoot = contentRoot;
        }

        public string ContentRoot { get; }
    }
}