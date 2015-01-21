using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SharpMCRewrite
{
    public class MSGBuffer
    {
        private ClientWrapper _Client;
        public int Size = 0;
        private int LastByte = 0;
        public byte[] BufferedData = new byte[4096];

        public MSGBuffer (ClientWrapper client)
        {
            _Client = client;
            mStream = client.TCPClient.GetStream ();
        }

        #region Reader
        private int ReadByte()
        {
            byte returnData = BufferedData [LastByte];
            LastByte++;
            return returnData;
        }

        private byte[] Read(int Length)
        {
            byte[] Buffered = new byte[Length];
            Array.Copy (BufferedData, LastByte, Buffered, 0, Length);
            LastByte += Length;
            return Buffered;
        }

        public int ReadVarInt()
        {
            int value = 0;
            int size = 0;
            int b;
            while (((b = ReadByte()) & 0x80) == 0x80) {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5) {
                    throw new IOException("VarInt too long. Hehe that's punny.");
                }
            }
            return value | ((b & 0x7F) << (size * 7));
        }

        public long ReadVarLong()
        {
            int value = 0;
            int size = 0;
            int b;
            while (((b = ReadByte()) & 0x80) == 0x80) {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 10) {
                    throw new IOException("VarLong too long. That's what she said.");
                }
            }
            return value | ((b & 0x7F) << (size * 7));
        }

        public short ReadShort()
        {
            int o = ReadByte ();
            int i = ReadByte ();

            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt16(new byte[2] {(byte)i , (byte)o }, 0);
            else
                return BitConverter.ToInt16(new byte[2] {(byte)o , (byte)i }, 0);

        }

        public string ReadString()
        {
            int Length = ReadVarInt ();
            byte[] StringValue = Read (Length);
            return Encoding.UTF8.GetString (StringValue);
        }

        /// <summary>
        /// Reads the username. (We cannot just use ReadString() because of some weird bug)...
        /// Idk what happend, but it seems to send an extra byte for the username there...
        /// </summary>
        /// <returns>The username.</returns>
        public string ReadUsername()
        {
            byte[] NoEdit = Encoding.UTF8.GetBytes(ReadString ());
            List<byte> t = new List<byte> ();

            int D = 0;
            foreach (byte i in NoEdit)
            {
                if (D > 1)
                {
                    t.Add (i);
                }
                D++;
            }
            return Encoding.UTF8.GetString(t.ToArray());
        }
        #endregion

        #region Writer
        private List<byte> bffr = new List<byte>();
        private NetworkStream mStream;

        public void Write(byte[] Data, int Offset, int Length)
        {
            for (int i = 0; i < Length; i++)
            {
                bffr.Add (Data [i + Offset]);
            }
        }

        public void Write(byte[] Data)
        {
            foreach (byte i in Data)
            {
                bffr.Add (i);
            }
        }

        public void WriteVarInt(int Integer)
        {
            while ((Integer & -128) != 0)
            {
                bffr.Add((byte)(Integer & 127 | 128));
                Integer = (int)(((uint)Integer) >> 7);
            }
            bffr.Add((byte)Integer);
        }

        public void WriteVarLong(long i)
        {
            long Fuck = i;
            while ((Fuck & ~0x7F) != 0) {
                bffr.Add((byte)((Fuck & 0x7F) | 0x80));
                Fuck >>= 7;
            }
            bffr.Add((byte)Fuck);
        }

        public void WriteInt(int Data)
        {
            byte[] Buffer = BitConverter.GetBytes (Data);
            Write (Buffer);
        }

        public void WriteString(string Data)
        {
            byte[] StringData = Encoding.UTF8.GetBytes (Data);
            WriteVarInt (StringData.Length);
            Write (StringData);
        }

        public void WriteShort(short Data)
        {
            byte[] ShortData = BitConverter.GetBytes (Data);
            Write (ShortData);
        }

        public void WriteByte(byte Data)
        {
            bffr.Add (Data);
        }

        public void WriteBool(bool Data)
        {
            Write(BitConverter.GetBytes (Data));
        }

        public void WriteDouble(double Data)
        {
            Write (BitConverter.GetBytes (Data));
        }

        public void WriteFloat(float Data)
        {
            Write (BitConverter.GetBytes (Data));
        }

        public void WriteLong(long Data)
        {
            Write (BitConverter.GetBytes (Data));
        }

        /// <summary>
        /// Flush all data to the TCPClient NetworkStream.
        /// </summary>
        public void FlushData()
        {
            try
            {
                byte[] AllData = bffr.ToArray ();
                bffr.Clear ();

                WriteVarInt (AllData.Length);
                byte[] Buffer = bffr.ToArray ();

                ConsoleFunctions.WriteDebugLine ("Specified Data length: " + AllData.Length);
                ConsoleFunctions.WriteDebugLine ("Full packet length: " + (AllData.Length + Buffer.Length));
                mStream.Write (Buffer, 0, Buffer.Length);
                mStream.Write (AllData, 0, AllData.Length);
                bffr.Clear ();
            }
            catch
            {
                ConsoleFunctions.WriteErrorLine ("Failed to send a packet!");
            }
        }

        #endregion
    }
}

