using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class InitializeWorldBorder : Packet<InitializeWorldBorder>, IToClient
    {
        public byte ClientId => 0x20;

        public double X { get; set; }
        public double Z { get; set; }
        public double OldDiameter { get; set; }
        public double NewDiameter { get; set; }
        public int Speed { get; set; }
        public int PortalTeleportBoundary { get; set; }
        public int WarningBlocks { get; set; }
        public int WarningTime { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            X = stream.ReadDouble();
            Z = stream.ReadDouble();
            OldDiameter = stream.ReadDouble();
            NewDiameter = stream.ReadDouble();
            Speed = stream.ReadVarInt();
            PortalTeleportBoundary = stream.ReadVarInt();
            WarningBlocks = stream.ReadVarInt();
            WarningTime = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Z);
            stream.WriteDouble(OldDiameter);
            stream.WriteDouble(NewDiameter);
            stream.WriteVarInt(Speed);
            stream.WriteVarInt(PortalTeleportBoundary);
            stream.WriteVarInt(WarningBlocks);
            stream.WriteVarInt(WarningTime);
        }
    }
}
