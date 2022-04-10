using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WorldEvent : Packet<WorldEvent>, IToClient
    {
        public byte ClientId => 0x23;

        public int EffectId { get; set; }
        public System.Numerics.Vector3 Location { get; set; }
        public int Data { get; set; }
        public bool Global { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EffectId = stream.ReadInt();
            Location = stream.ReadPosition();
            Data = stream.ReadInt();
            Global = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(EffectId);
            stream.WritePosition(Location);
            stream.WriteInt(Data);
            stream.WriteBool(Global);
        }
    }
}
