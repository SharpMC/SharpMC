using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntitySoundEffect : Packet<EntitySoundEffect>, IToClient
    {
        public byte ClientId => 0x5c;

        public int SoundId { get; set; }
        public int SoundCategory { get; set; }
        public int EntityId { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            SoundId = stream.ReadVarInt();
            SoundCategory = stream.ReadVarInt();
            EntityId = stream.ReadVarInt();
            Volume = stream.ReadFloat();
            Pitch = stream.ReadFloat();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(SoundId);
            stream.WriteVarInt(SoundCategory);
            stream.WriteVarInt(EntityId);
            stream.WriteFloat(Volume);
            stream.WriteFloat(Pitch);
        }
    }
}
