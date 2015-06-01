using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SharpMC.Utils
{
	/// <summary>
	/// Currently only used for reading.
	/// </summary>
	public class MCStream : NetworkStream
	{
		public int TotalDataLength = 0;
		public MCStream(Socket socket) : base(socket)
		{
		}

		public MCStream(Socket socket, bool ownsSocket) : base(socket, ownsSocket)
		{
		}

		public MCStream(Socket socket, FileAccess access) : base(socket, access)
		{
		}

		public MCStream(Socket socket, FileAccess access, bool ownsSocket) : base(socket, access, ownsSocket)
		{
		
		}

		public void FlushData(bool quee = false)
		{
			Flush();
		}

		#region Read
		public byte[] Read(int Length)
		{
			var Buffered = new byte[Length];
			Read(Buffered, 0, Length);
			if (BitConverter.IsLittleEndian) Array.Reverse(Buffered);

			return Buffered;
		}

		public int ReadInt()
		{
			var Dat = Read(4);
			var Value = BitConverter.ToInt32(Dat, 0);
			return Value;
			//return IPAddress.NetworkToHostOrder(Value);
		}

		public float ReadFloat()
		{
			var Almost = Read(4);
			var f = BitConverter.ToSingle(Almost, 0);
			//return NetworkToHostOrder(f);
			return f;
		}

		public bool ReadBool()
		{
			var Answer = ReadByte();
			if (Answer == 1)
				return true;
			return false;
		}

		public double ReadDouble()
		{
			var AlmostValue = Read(8);
			//return NetworkToHostOrder(AlmostValue);
			return BitConverter.ToDouble(AlmostValue, 0);
		}

		public int ReadVarInt()
		{
			var value = 0;
			var size = 0;
			int b;
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++ * 7);
				if (size > 5)
				{
					throw new IOException("VarInt too long. Hehe that's punny.");
				}
			}
			return value | ((b & 0x7F) << (size * 7));
		}

		public long ReadVarLong()
		{
			var value = 0;
			var size = 0;
			int b;
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++ * 7);
				if (size > 10)
				{
					throw new IOException("VarLong too long. That's what she said.");
				}
			}
			return value | ((b & 0x7F) << (size * 7));
		}

		public short ReadShort()
		{
			var Da = Read(2);
			var D = BitConverter.ToInt16(Da, 0);
			//return IPAddress.NetworkToHostOrder(D);
			return D;
		}

		public ushort ReadUShort()
		{
			var Da = Read(2);
			//return NetworkToHostOrder(BitConverter.ToUInt16(Da, 0));
			return BitConverter.ToUInt16(Da, 0);
		}

		public ushort[] ReadUShort(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var Da = Read(2);
				var D = BitConverter.ToUInt16(Da, 0);
				us[i] = D;
			}
			//return NetworkToHostOrder(us);
			return us;
			//return IPAddress.NetworkToHostOrder (D);
		}

		public ushort[] ReadUShortLocal(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var Da = Read(2);
				var D = BitConverter.ToUInt16(Da, 0);
				us[i] = D;
			}
			return us;
			//return IPAddress.NetworkToHostOrder (D);
		}

		public short[] ReadShortLocal(int count)
		{
			var us = new short[count];
			for (var i = 0; i < us.Length; i++)
			{
				var Da = Read(2);
				var D = BitConverter.ToInt16(Da, 0);
				us[i] = D;
			}
			return us;
			//return IPAddress.NetworkToHostOrder (D);
		}

		public string ReadString()
		{
			var Length = ReadVarInt();
			var StringValue = Read(Length);
			return Encoding.UTF8.GetString(StringValue);
		}

		public long ReadLong()
		{
			var l = Read(8);
			//return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(l, 0));
			return BitConverter.ToInt64(l, 0);
		}

		public Vector3 ReadPosition()
		{
			var val = ReadLong();
			var x = Convert.ToDouble(val >> 38);
			var y = Convert.ToDouble((val >> 26) & 0xFFF);
			var z = Convert.ToDouble(val << 38 >> 38);
			return new Vector3(x, y, z);
		}

		private double NetworkToHostOrder(byte[] data)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
			}
			return BitConverter.ToDouble(data, 0);
		}

		private float NetworkToHostOrder(float network)
		{
			var bytes = BitConverter.GetBytes(network);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return BitConverter.ToSingle(bytes, 0);
		}

		private ushort[] NetworkToHostOrder(ushort[] network)
		{
			if (BitConverter.IsLittleEndian)
				Array.Reverse(network);
			return network;
		}

		private ushort NetworkToHostOrder(ushort network)
		{
			var net = BitConverter.GetBytes(network);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(net);
			return BitConverter.ToUInt16(net, 0);
		}
		#endregion

		#region Write

		public void Write(byte[] Data)
		{
			foreach (var i in Data)
			{
				WriteByte(i);
			}
		}

		public void WritePosition(Vector3 position)
		{
			var x = Convert.ToInt64(position.X);
			var y = Convert.ToInt64(position.Y);
			var z = Convert.ToInt64(position.Z);
			var toSend = ((x & 0x3FFFFFF) << 38) | ((y & 0xFFF) << 26) | (z & 0x3FFFFFF);
			WriteLong(toSend);
		}

		public void WriteVarInt(int Integer)
		{
			while ((Integer & -128) != 0)
			{
				WriteByte((byte)(Integer & 127 | 128));
				Integer = (int)(((uint)Integer) >> 7);
			}
			WriteByte((byte)Integer);
		}

		public void WriteVarLong(long i)
		{
			var Fuck = i;
			while ((Fuck & ~0x7F) != 0)
			{
				WriteByte((byte)((Fuck & 0x7F) | 0x80));
				Fuck >>= 7;
			}
			WriteByte((byte)Fuck);
		}

		public void WriteInt(int Data)
		{
			var Buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data));
			Write(Buffer);
		}

		public void WriteString(string Data)
		{
			var StringData = Encoding.UTF8.GetBytes(Data);
			WriteVarInt(StringData.Length);
			Write(StringData);
		}

		public void WriteShort(short Data)
		{
			var ShortData = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data));
			Write(ShortData);
		}

		public void WriteUShort(ushort Data)
		{
			var UShortData = BitConverter.GetBytes(Data);
			Write(UShortData);
		}

		public void WriteBool(bool Data)
		{
			Write(BitConverter.GetBytes(Data));
		}

		public void WriteDouble(double Data)
		{
			Write(HostToNetworkOrder(Data));
		}

		public void WriteFloat(float Data)
		{
			Write(HostToNetworkOrder(Data));
		}

		public void WriteLong(long Data)
		{
			Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data)));
		}

		public void WriteUUID(Guid uuid)
		{
			var guid = uuid.ToByteArray();
			var Long1 = new byte[8];
			var Long2 = new byte[8];
			Array.Copy(guid, 0, Long1, 0, 8);
			Array.Copy(guid, 8, Long2, 0, 8);
			Write(Long1);
			Write(Long2);
		}

		private byte[] HostToNetworkOrder(double d)
		{
			var data = BitConverter.GetBytes(d);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(data);

			return data;
		}

		private byte[] HostToNetworkOrder(float host)
		{
			var bytes = BitConverter.GetBytes(host);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return bytes;
		}

		#endregion
	}
}
