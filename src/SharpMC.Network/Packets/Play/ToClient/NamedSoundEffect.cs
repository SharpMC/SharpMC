using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class NamedSoundEffect : Packet<NamedSoundEffect>, IToClient
    {
        public byte ClientId => 0x19;

        public string SoundName { get; set; }
        public int SoundCategory { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            SoundName = stream.ReadString();
            SoundCategory = stream.ReadVarInt();
            X = stream.ReadInt();
            Y = stream.ReadInt();
            Z = stream.ReadInt();
            Volume = stream.ReadFloat();
            Pitch = stream.ReadFloat();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(SoundName);
            stream.WriteVarInt(SoundCategory);
            stream.WriteInt(X);
            stream.WriteInt(Y);
            stream.WriteInt(Z);
            stream.WriteFloat(Volume);
            stream.WriteFloat(Pitch);
        }
    }
}
