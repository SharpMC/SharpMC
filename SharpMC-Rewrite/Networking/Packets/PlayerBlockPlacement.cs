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
            Position.Y++;//Temporaily!

            Debug.WriteLine (Position.GetString ());

            int Direction = buffer.ReadByte ();
            short HeldItem = buffer.ReadShort (); // I guess?
            int CursorX = buffer.ReadByte ();
            int CursorY = buffer.ReadByte ();
            int CursorZ = buffer.ReadByte ();

            Globals.Level.Generator.SetBlock (Position, (ushort)HeldItem);
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

