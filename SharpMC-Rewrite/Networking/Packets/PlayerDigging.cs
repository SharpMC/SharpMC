using System;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Enums;

namespace SharpMCRewrite
{
    public class PlayerDigging : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x07;
            }
        }

        public bool IsPlayePacket
        {
            get
            {
                return true;
            }
        }

        public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            ConsoleFunctions.WriteDebugLine ("Received 'Player Digging'");
            int Status = buffer.ReadByte ();
            Vector3 Position = buffer.ReadPosition ();
            int Face = buffer.ReadByte ();
			INTVector3 intVector = new INTVector3((int) Position.X, (int) Position.Y, (int) Position.Z);

            Globals.Level.SetBlock (new BlockAir() {Coordinates = intVector});
			Console.WriteLine("Block pos: " + intVector.GetString());
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

