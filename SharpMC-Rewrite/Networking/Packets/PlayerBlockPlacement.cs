using System;
using System.Diagnostics;
using SharpMCRewrite.Blocks;

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

            ushort HeldItem = buffer.ReadUShort(); // I guess?
           // int Type = HeldItem >> 5;
	        byte itemCount = (byte)buffer.ReadByte();
			short itemDamage = buffer.ReadShort();
			byte itemMeta = (byte)buffer.ReadByte();

	        ushort meta = 0;
	        switch (@itemMeta)
	        {
				case 0:
					//NO NBT Shizzle!
					Console.WriteLine("No NBT");
			        break;
				case 1:
					//Byte
					Console.WriteLine("Byte");
			        break;
				case 2:
					//short
					Console.WriteLine("Short");
			        break;
				default:
					Console.WriteLine("Default :(");
			        break;
	        }

            int CursorX = buffer.ReadByte ();
            int CursorY = buffer.ReadByte ();
            int CursorZ = buffer.ReadByte ();

			INTVector3 intVector = new INTVector3((int) Position.X, (int) Position.Y, (int) Position.Z);
	        Block b = BlockFactory.GetBlockById(HeldItem);
	        b.Coordinates = intVector;
	        b.Metadata = meta;
			Globals.Level.SetBlock (b);
           // Globals.Level.BroadcastPacket (new BlockChange (), new object[] { Position, (int)HeldItem, (int)0 });
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

