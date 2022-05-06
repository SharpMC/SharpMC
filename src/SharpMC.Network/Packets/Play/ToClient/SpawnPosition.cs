﻿using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SpawnPosition : Packet<SpawnPosition>, IToClient
    {
        public byte ClientId => 0x4b;

        public Vector3 Location { get; set; }
        public float Angle { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Angle = stream.ReadFloat();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteFloat(Angle);
        }
    }
}
