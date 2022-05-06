using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpMC.API;
using SharpMC.API.Worlds;
using SharpMC.Config;
using SharpMC.Util;

namespace SharpMC.Meta
{
    public class ServerInfo : IServerInfo
    {
        #region Constants

        private const int Protocol = 758;
        internal const string ProtocolName = "1.18.2";

        #endregion

        private readonly ILevelManager _levelManager;
        private readonly ServerSettings _settings;
        private readonly IHostEnv _env;

        public ServerInfo(ILevelManager levelManager, ServerSettings settings, IHostEnv env)
        {
            _levelManager = levelManager;
            _settings = settings;
            _env = env;
        }

        public string GetMotd()
        {
            var message = new MetaServer
            {
                Version = new MetaVersion {Name = ProtocolName, Protocol = Protocol},
                Players = new MetaPlayers
                {
                    Max = _settings.General?.MaxPlayers ?? 1,
                    Online = Players,
                    Sample = new List<MetaSample>
                    {
                        new() {Id = Guid.NewGuid().ToString(), Name = "john_doe"}
                    }
                },
                Description = new MetaDescription {Text = _settings.General?.Motd ?? "???"}
            };
            if (_settings.General?.FavIcon is { } favIcon)
            {
                var path = Path.Combine(_env.ContentRoot, favIcon);
                var favBytes = File.ReadAllBytes(path);
                message.Favicon = $"data:image/png;base64,{Convert.ToBase64String(favBytes)}";
            }
            var json = JsonHelper.ToJson(message);
            return json;
        }

        private int Players => _levelManager.GetLevels().Sum(x => x.PlayerCount);
    }
}