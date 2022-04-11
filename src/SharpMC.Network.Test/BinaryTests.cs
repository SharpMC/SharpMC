using SharpMC.Network.Packets.Play.ToClient;
using Xunit;
using static SharpMC.Network.Test.TestHelper;

namespace SharpMC.Network.Test
{
    public class BinaryTests
    {
        [Fact]
        public void ShouldReadPlayLogin()
        {
            var input = DataBunch.PlayLogin;
            Assert.Equal(23992, input.Length);

            var packet = Read<Login>(input, out var packetId);
            Assert.Equal(0x26, packetId);

            Assert.Equal(167, packet.EntityId);
            Assert.False(packet.IsHardcore);
            Assert.Equal(0, packet.GameMode);
            Assert.Equal(-1, packet.PreviousGameMode);
            Assert.Equal(new[]
            {
                "minecraft:overworld", "minecraft:the_nether", "minecraft:the_end"
            }, packet.WorldNames);
        }

        [Fact]
        public void ShouldWritePlayLogin()
        {
            var expected = DataBunch.PlayLogin;
            Assert.Equal(23992, expected.Length);

            var packet = new Login
            {
                EntityId = 167,
                IsHardcore = false,
                GameMode = 0,
                PreviousGameMode = -1,
                WorldNames = new[]
                {
                    "minecraft:overworld", "minecraft:the_nether", "minecraft:the_end"
                },
                WorldName = "minecraft:overworld",
                HashedSeeds = new[] {-660566458, -1901654650},
                MaxPlayers = 20,
                ViewDistance = 10,
                SimulationDistance = 10,
                ReducedDebugInfo = false,
                EnableRespawnScreen = true,
                IsDebug = false,
                IsFlat = false
            };

            var actual = Write(packet, 0x26);
            WriteBytes(nameof(ShouldWritePlayLogin), expected, actual);
            Assert.Equal(ToJson(expected), ToJson(actual));
        }
    }
}