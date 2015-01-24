using System.Diagnostics;

namespace SharpMCRewrite
{
    public class PlayerBlockPlacement : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x08;
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
            Vector3 Position = buffer.ReadPosition ();
           //Position.Y++;//Temporaily!

            if (Position.Y > 256)
                return;

            int Direction = buffer.ReadByte ();
                
            switch (Direction)
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

            short HeldItem = buffer.ReadShort (); // I guess?
            int Type = HeldItem >> 5;

            switch (Type)
            {
                case 0:
                    //BYTE
                    break;
                case 1:
                    //Short
                    break;
                case 2:
                    //Int
                    break;
                case 3:
                    //Float
                    break;
                case 4:
                    //string
                    break;
                case 5:
                    //Slot
                    break;
                case 6:
                    //Vector
                    break;
                case 7:
                    //rotation
                    break;
            }

            int CursorX = buffer.ReadByte ();
            int CursorY = buffer.ReadByte ();
            int CursorZ = buffer.ReadByte ();

            Globals.Level.Generator.SetBlock (Position, (ushort)HeldItem);
            Globals.Level.BroadcastPacket (new BlockChange (), new object[] { Position, (int)HeldItem, (int)0 });
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

