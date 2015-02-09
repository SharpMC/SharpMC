using SharpMCRewrite.Blocks;

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
            switch (Face)
            {
                case 0:
                    intVector.Y--;
                    break;
                case 1:
                    intVector.Y++;
                    break;
                case 2:
                    intVector.Z--;
                    break;
                case 3:
                    intVector.Z++;
                    break;
                case 4:
                    intVector.X--;
                    break;
                case 5:
                    intVector.X++;
                    break;

            }
            Globals.Level.SetBlock (new BlockAir() {Coordinates = intVector});
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

