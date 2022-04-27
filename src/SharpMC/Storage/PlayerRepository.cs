using System.IO;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.Network.Util;
using SharpMC.Players;
using SharpMC.Storage.API;

namespace SharpMC.Storage
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly string _folder;
        private readonly ICompression _compression;

        private readonly dynamic _healthManager;
        private readonly dynamic _inventory;

        public PlayerRepository(string folder, ICompression compression)
        {
            _compression = compression;
            _folder = Path.Combine(folder, "Players");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public bool OnlineMode { get; set; }

        public void SavePlayer(IPlayer player)
        {
            byte[] health = _healthManager.Export();
            byte[] inv = _inventory.Export();
            using var mem = new MemoryStream();
            using var buffer = new MinecraftStream(mem);
            var pos = player.KnownPosition;
            buffer.WriteDouble(pos.X);
            buffer.WriteDouble(pos.Y);
            buffer.WriteDouble(pos.Z);
            buffer.WriteFloat((float) pos.Yaw);
            buffer.WriteFloat(pos.Pitch);
            buffer.WriteBool(pos.OnGround);
            buffer.WriteVarInt((int) player.Gamemode);
            buffer.WriteVarInt(health.Length);
            foreach (var b in health)
            {
                buffer.WriteByte(b);
            }
            buffer.WriteVarInt(inv.Length);
            foreach (var b in inv)
            {
                buffer.WriteByte(b);
            }
            var data = mem.ToArray();
            data = _compression.Compress(data);
            var saveName = OnlineMode ? player.Uuid.ToString("N") : player.Username;
            var file = GetFileName(_folder, saveName);
            File.WriteAllBytes(file, data);
        }

        public bool Exists(string saveName)
        {
            return File.Exists(GetFileName(_folder, saveName));
        }

        public IPlayer LoadPlayer((string uuid, string userName) id, dynamic level)
        {
            var saveName = OnlineMode ? id.uuid : id.userName;
            var player = new Player(null, null, null);
            var file = GetFileName(_folder, saveName);
            if (File.Exists(file))
            {
                var data = File.ReadAllBytes(file);
                data = _compression.Decompress(data);
                using var mem = new MemoryStream(data);
                using var reader = new MinecraftStream(mem);
                var x = reader.ReadDouble();
                var y = reader.ReadDouble();
                var z = reader.ReadDouble();
                var yaw = reader.ReadFloat();
                var pitch = reader.ReadFloat();
                var onGround = reader.ReadBool();
                player.KnownPosition = new PlayerLocation(x, y, z)
                {
                    Yaw = yaw, Pitch = pitch, OnGround = onGround
                };
                player.Gamemode = (GameMode) reader.ReadVarInt();
                var healthLength = reader.ReadVarInt();
                var healthData = reader.Read(healthLength);
                var inventoryLength = reader.ReadVarInt();
                var inventoryData = reader.Read(inventoryLength);
                _healthManager.Import(healthData);
                _inventory.Import(inventoryData);
            }
            else
            {
                player.KnownPosition = level.SpawnPoint;
            }
            return (IPlayer) player;
        }

        private static string GetFileName(string folder, string saveName)
        {
            return $"{folder}/{saveName}.pdata";
        }
    }
}