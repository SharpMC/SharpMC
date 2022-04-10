using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class SteerVehicle : Packet<SteerVehicle>, IToServer
    {
        public byte ServerId => 0x1c;

        public float Sideways { get; set; }
        public float Forward { get; set; }
        public byte Jump { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Sideways = stream.ReadFloat();
            Forward = stream.ReadFloat();
            Jump = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteFloat(Sideways);
            stream.WriteFloat(Forward);
            stream.WriteByte(Jump);
        }
    }
}
