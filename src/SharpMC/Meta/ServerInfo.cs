using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharpMC.Meta;
using SharpMC.Util;

namespace SharpMC
{
    public class ServerInfo
    {
        private readonly MinecraftServer _server;

        public ServerInfo(MinecraftServer server)
        {
            _server = server;
        }

        private const int Protocol = 758;
        internal const string ProtocolName = "1.18.2";

        public string Motd { get; set; } = "A SharpMC Server";
        public int MaxPlayers { get; set; } = 30;

        public int Players => _server.LevelManager.GetLevels().Sum(x => x.PlayerCount);

        public string GetMotd()
        {
            var message = new MetaServer
            {
                Version = new MetaVersion {Name = ProtocolName, Protocol = Protocol},
                Players = new MetaPlayers
                {
                    Max = MaxPlayers, Online = Players, Sample = new List<MetaSample>
                    {
                        new MetaSample {Id = Guid.NewGuid().ToString(), Name = "johndoe"}
                    }
                },
                Description = new MetaDescription {Text = Motd}
            };
            var json = JsonHelper.ToJson(message);
            return json;
        }
    }
}