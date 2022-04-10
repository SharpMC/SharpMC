using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class UpdateJigsawBlock : Packet<UpdateJigsawBlock>, IToServer
    {
        public byte ServerId => 0x29;

        public System.Numerics.Vector3 Location { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public string Pool { get; set; }
        public string FinalState { get; set; }
        public string JointType { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Name = stream.ReadString();
            Target = stream.ReadString();
            Pool = stream.ReadString();
            FinalState = stream.ReadString();
            JointType = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteString(Name);
            stream.WriteString(Target);
            stream.WriteString(Pool);
            stream.WriteString(FinalState);
            stream.WriteString(JointType);
        }
    }
}
