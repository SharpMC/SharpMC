using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SharpMCRewrite
{
	public class MSGBuffer
	{
		private readonly ClientWrapper _Client;
		public byte[] BufferedData = new byte[4096*2];
		private int LastByte;
		public int Size = 0;

		public MSGBuffer(ClientWrapper client)
		{
			_Client = client;
			if (client.TCPClient.Connected) mStream = client.TCPClient.GetStream();
		}

		public MSGBuffer(NetworkStream stream)
		{
			mStream = stream;
		}

		public MSGBuffer(byte[] Data)
		{
			BufferedData = Data;
		}

		#region Reader

		public int ReadByte()
		{
			var returnData = BufferedData[LastByte];
			LastByte++;
			return returnData;
		}

		public byte[] Read(int Length)
		{
			var Buffered = new byte[Length];
			Array.Copy(BufferedData, LastByte, Buffered, 0, Length);
			LastByte += Length;
			return Buffered;
		}


		public int ReadInt()
		{
			var Dat = Read(4);
			var Value = BitConverter.ToInt32(Dat, 0);
			return IPAddress.NetworkToHostOrder(Value);
		}

		public float ReadFloat()
		{
			var Almost = Read(4);
			var f = BitConverter.ToSingle(Almost, 0);
			return NetworkToHostOrder(f);
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
			return NetworkToHostOrder(AlmostValue);
		}

		public int ReadVarInt()
		{
			var value = 0;
			var size = 0;
			int b;
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++*7);
				if (size > 5)
				{
					throw new IOException("VarInt too long. Hehe that's punny.");
				}
			}
			return value | ((b & 0x7F) << (size*7));
		}

		public long ReadVarLong()
		{
			var value = 0;
			var size = 0;
			int b;
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++*7);
				if (size > 10)
				{
					throw new IOException("VarLong too long. That's what she said.");
				}
			}
			return value | ((b & 0x7F) << (size*7));
		}

		public short ReadShort()
		{
			var Da = Read(2);
			var D = BitConverter.ToInt16(Da, 0);
			return IPAddress.NetworkToHostOrder(D);
		}

		public ushort ReadUShort()
		{
			var Da = Read(2);
			return NetworkToHostOrder(BitConverter.ToUInt16(Da, 0));
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
			return NetworkToHostOrder(us);
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

		public string ReadString()
		{
			var Length = ReadVarInt();
			var StringValue = Read(Length);
			return Encoding.UTF8.GetString(StringValue);
		}

		public long ReadLong()
		{
			var l = Read(8);
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(l, 0));
		}

		public Vector3 ReadPosition()
		{
			var val = ReadLong();
			var x = Convert.ToDouble(val >> 38);
			var y = Convert.ToDouble((val >> 26) & 0xFFF);
			var z = Convert.ToDouble(val << 38 >> 38);
			return new Vector3(x, y, z);
		}

		public IntVector3 ReadIntPosition()
		{
			var val = ReadLong();
			var x = (int) Math.Floor((decimal) (val >> 38));
			var y = (int) Math.Floor((decimal) ((val >> 26) & 0xFFF));
			var z = (int) Math.Floor((decimal) (val << 38 >> 38));
			return new IntVector3(x, y, z);
		}

		/// <summary>
		///     Reads the username. (We cannot just use ReadString() because of some weird bug)...
		///     Idk what happend, but it seems to send an extra byte for the username there...
		/// </summary>
		/// <returns>The username.</returns>
		public string ReadUsername()
		{
			var NoEdit = Encoding.UTF8.GetBytes(ReadString());
			var t = new List<byte>();

			var D = 0;
			foreach (var i in NoEdit)
			{
				if (D > 1)
				{
					t.Add(i);
				}
				D++;
			}
			return Encoding.UTF8.GetString(t.ToArray());
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

		#region Writer

		public byte[] ExportWriter
		{
			get { return bffr.ToArray(); }
		}

		private readonly List<byte> bffr = new List<byte>();
		private readonly NetworkStream mStream;

		public void Write(byte[] Data, int Offset, int Length)
		{
			for (var i = 0; i < Length; i++)
			{
				bffr.Add(Data[i + Offset]);
			}
		}

		public void Write(byte[] Data)
		{
			foreach (var i in Data)
			{
				bffr.Add(i);
			}
		}

		public void WritePosition(Vector3 Position)
		{
			Position.ConvertToNetwork();
			long ToSend = (((((int) Position.X) & 0x3FFFFFF) << 38) | ((((int) Position.Y) & 0xFFF) << 26) |
			               (((int) Position.Z) & 0x3FFFFFF));
			WriteLong(ToSend);
		}

		public void WritePosition(IntVector3 Position)
		{
			Position.ConvertToNetwork();
			long toSend = ((((Position.X) & 0x3FFFFFF) << 38) | (((Position.Y) & 0xFFF) << 26) | ((Position.Z) & 0x3FFFFFF));
			WriteLong(toSend);
		}

		public void WriteVarInt(int Integer)
		{
			while ((Integer & -128) != 0)
			{
				bffr.Add((byte) (Integer & 127 | 128));
				Integer = (int) (((uint) Integer) >> 7);
			}
			bffr.Add((byte) Integer);
		}

		public void WriteVarLong(long i)
		{
			var Fuck = i;
			while ((Fuck & ~0x7F) != 0)
			{
				bffr.Add((byte) ((Fuck & 0x7F) | 0x80));
				Fuck >>= 7;
			}
			bffr.Add((byte) Fuck);
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
			var ShortData = BitConverter.GetBytes(Data);
			Write(ShortData);
		}

		public void WriteUShort(ushort Data)
		{
			var UShortData = BitConverter.GetBytes(Data);
			Write(UShortData);
		}

		public void WriteByte(byte Data)
		{
			bffr.Add(Data);
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

		/// <summary>
		///     Flush all data to the TCPClient NetworkStream.
		/// </summary>
		public void FlushData(bool quee = false)
		{
			try
			{
				var AllData = bffr.ToArray();
				bffr.Clear();

				WriteVarInt(AllData.Length);
				var Buffer = bffr.ToArray();

				// ConsoleFunctions.WriteDebugLine ("Specified Data length: " + AllData.Length);
				//  ConsoleFunctions.WriteDebugLine ("Full packet length: " + (AllData.Length + Buffer.Length));
//                mStream.Write (Buffer, 0, Buffer.Length);
				//              mStream.Write (AllData, 0, AllData.Length);
				var data = new List<byte>();
				foreach (var i in Buffer)
				{
					data.Add(i);
				}
				foreach (var i in AllData)
				{
					data.Add(i);
				}
				_Client.AddToQuee(data.ToArray(), quee);
				bffr.Clear();
			}
			catch (Exception ex)
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!\n" + ex);
			}
		}

		public void FlushData(int packetId)
		{
			try
			{
				var AllData = bffr.ToArray();
				bffr.Clear();

				WriteVarInt(packetId);
				WriteVarInt(AllData.Length);
				var Buffer = bffr.ToArray();

				// ConsoleFunctions.WriteDebugLine ("Specified Data length: " + AllData.Length);
				//  ConsoleFunctions.WriteDebugLine ("Full packet length: " + (AllData.Length + Buffer.Length));
				mStream.Write(Buffer, 0, Buffer.Length);
				mStream.Write(AllData, 0, AllData.Length);
				bffr.Clear();
			}
			catch (Exception ex)
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!\n" + ex);
			}
		}

		private byte[] HostToNetworkOrder(double d)
		{
			var data = BitConverter.GetBytes(d);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
			}
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