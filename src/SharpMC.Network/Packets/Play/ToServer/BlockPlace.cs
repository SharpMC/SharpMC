using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class BlockPlace : Packet<BlockPlace>, IToServer
    {
        public byte ServerId => 0x2e;

        public int Hand { get; set; }
        public Vector3 Location { get; set; }
        public int Direction { get; set; }
        public float CursorX { get; set; }
        public float CursorY { get; set; }
        public float CursorZ { get; set; }
        public bool InsideBlock { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Hand = stream.ReadVarInt();
            Location = stream.ReadPosition();
            Direction = stream.ReadVarInt();
            CursorX = stream.ReadFloat();
            CursorY = stream.ReadFloat();
            CursorZ = stream.ReadFloat();
            InsideBlock = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Hand);
            stream.WritePosition(Location);
            stream.WriteVarInt(Direction);
            stream.WriteFloat(CursorX);
            stream.WriteFloat(CursorY);
            stream.WriteFloat(CursorZ);
            stream.WriteBool(InsideBlock);
        }
    }
}
