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
            int Status = buffer.ReadByte ();
            Vector3 Position = buffer.ReadPosition ();
            int Face = buffer.ReadByte ();
            switch (Face)
            {
                case 0:
                    Position.Y--;
                    break;
                case 1:
                    Position.Y++;
                    break;
                case 2:
                    Position.Z--;
                    break;
                case 3:
                    Position.Z++;
                    break;
                case 4:
                    Position.X--;
                    break;
                case 5:
                    Position.X++;
                    break;

            }
            Globals.Level.Generator.SetBlock (Position, 0);
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

