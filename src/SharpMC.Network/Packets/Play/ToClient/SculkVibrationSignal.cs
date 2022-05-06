using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SculkVibrationSignal : Packet<SculkVibrationSignal>, IToClient
    {
        public byte ClientId => 0x05;

        public Vector3 SourcePosition { get; set; }
        public string DestinationIdentifier { get; set; }
        public int ArrivalTicks { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            SourcePosition = stream.ReadPosition();
            DestinationIdentifier = stream.ReadString();
            ArrivalTicks = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(SourcePosition);
            stream.WriteString(DestinationIdentifier);
            stream.WriteVarInt(ArrivalTicks);
        }
    }
}
