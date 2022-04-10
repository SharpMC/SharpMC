using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class BlockBreakAnimation : Packet<BlockBreakAnimation>, IToClient
    {
        public byte ClientId => 0x09;

        public int EntityId { get; set; }
        public System.Numerics.Vector3 Location { get; set; }
        public sbyte DestroyStage { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            Location = stream.ReadPosition();
            DestroyStage = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WritePosition(Location);
            stream.WriteSByte(DestroyStage);
        }
    }
}
