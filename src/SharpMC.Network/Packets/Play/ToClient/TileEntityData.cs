using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class TileEntityData : Packet<TileEntityData>, IToClient
    {
        public byte ClientId => 0x0a;

        public System.Numerics.Vector3 Location { get; set; }
        public int Action { get; set; }
        public object NbtData { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Action = stream.ReadVarInt();
            NbtData = stream.ReadOptNbt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteVarInt(Action);
            stream.WriteOptNbt(NbtData);
        }
    }
}
