// Distributed under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Ionic.Zlib;
using SharpMC.Core.Networking;

namespace SharpMC.Core.Utils
{
	public class LocalDataBuffer : DataBuffer
	{
		public LocalDataBuffer(ClientWrapper client) : base(client)
		{
		}

		public LocalDataBuffer(byte[] data) : base(data)
		{
		}
	}

	
	public class DataBuffer
	{
		private readonly ClientWrapper _client;
		public byte[] BufferedData = new byte[4096];
		private int _lastByte;
		public int Size = 0;

		public DataBuffer(ClientWrapper client)
		{
			_client = client;
		}

		public DataBuffer(byte[] data)
		{
			BufferedData = data;
		}

		public void SetDataSize(int size)
		{
			Array.Resize(ref BufferedData, size);
			Size = size;
		}

		public void Dispose()
		{
			BufferedData = null;
			_lastByte = 0;
		}

		#region Reader

		public int ReadByte()
		{
			var returnData = BufferedData[_lastByte];
			_lastByte++;
			return returnData;
		}

		public byte[] Read(int length)
		{
			var buffered = new byte[length];
			Array.Copy(BufferedData, _lastByte, buffered, 0, length);
			_lastByte += length;
			return buffered;
		}


		public int ReadInt()
		{
			var dat = Read(4);
			var value = BitConverter.ToInt32(dat, 0);
			return IPAddress.NetworkToHostOrder(value);
		}

		public float ReadFloat()
		{
			var almost = Read(4);
			var f = BitConverter.ToSingle(almost, 0);
			return NetworkToHostOrder(f);
		}

		public bool ReadBool()
		{
			var answer = ReadByte();
			if (answer == 1)
				return true;
			return false;
		}

		public double ReadDouble()
		{
			var almostValue = Read(8);
			return NetworkToHostOrder(almostValue);
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
			var da = Read(2);
			var d = BitConverter.ToInt16(da, 0);
			return IPAddress.NetworkToHostOrder(d);
		}

		public ushort ReadUShort()
		{
			var da = Read(2);
			return NetworkToHostOrder(BitConverter.ToUInt16(da, 0));
		}

		public ushort[] ReadUShort(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return NetworkToHostOrder(us);
		}

		public ushort[] ReadUShortLocal(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public short[] ReadShortLocal(int count)
		{
			var us = new short[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public string ReadString()
		{
			var length = ReadVarInt();
			var stringValue = Read(length);

			return Encoding.UTF8.GetString(stringValue);
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
			get { return _bffr.ToArray(); }
		}

		private readonly List<byte> _bffr = new List<byte>();

		public void Write(byte[] data, int offset, int length)
		{
			for (var i = 0; i < length; i++)
			{
				_bffr.Add(data[i + offset]);
			}
		}

		public void Write(byte[] data)
		{
			foreach (var i in data)
			{
				_bffr.Add(i);
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

		public void WriteVarInt(int integer)
		{
			while ((integer & -128) != 0)
			{
				_bffr.Add((byte) (integer & 127 | 128));
				integer = (int) (((uint) integer) >> 7);
			}
			_bffr.Add((byte) integer);
		}

		public void WriteVarLong(long i)
		{
			var fuck = i;
			while ((fuck & ~0x7F) != 0)
			{
				_bffr.Add((byte) ((fuck & 0x7F) | 0x80));
				fuck >>= 7;
			}
			_bffr.Add((byte) fuck);
		}

		public void WriteInt(int data)
		{
			var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(buffer);
		}

		public void WriteString(string data)
		{
			var stringData = Encoding.UTF8.GetBytes(data);
			WriteVarInt(stringData.Length);
			Write(stringData);
		}

		public void WriteShort(short data)
		{
			var shortData = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(shortData);
		}

		public void WriteUShort(ushort data)
		{
			var uShortData = BitConverter.GetBytes(data);
			Write(uShortData);
		}

		public void WriteByte(byte data)
		{
			_bffr.Add(data);
		}

		public void WriteBool(bool data)
		{
			Write(BitConverter.GetBytes(data));
		}

		public void WriteDouble(double data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteFloat(float data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteLong(long data)
		{
			Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));
		}

		public void WriteUuid(Guid uuid)
		{
			var guid = uuid.ToByteArray();
			var long1 = new byte[8];
			var long2 = new byte[8];
			Array.Copy(guid, 0, long1, 0, 8);
			Array.Copy(guid, 8, long2, 0, 8);
			Write(long1);
			Write(long2);
		}

		private byte[] GetVarIntBytes(int integer)
		{
			List<Byte> bytes = new List<byte>();
			while ((integer & -128) != 0)
			{
				bytes.Add((byte)(integer & 127 | 128));
				integer = (int)(((uint)integer) >> 7);
			}
			bytes.Add((byte)integer);
			return bytes.ToArray();
		}

		/// <summary>
		///     Flush all data to the TCPClient NetworkStream.
		/// </summary>
		public void FlushData(bool quee = false)
		{
			try
			{
				var allData = _bffr.ToArray();
				_bffr.Clear();

				if (ServerSettings.UseCompression && _client.PacketMode == PacketMode.Play && _client.SetCompressionSend)
				{
					bool isOver = (allData.Length >= ServerSettings.CompressionThreshold);
					int dataLength = isOver ? allData.Length : 0;

					//Calculate length of 'Data Length'
					byte[] dLength = GetVarIntBytes(dataLength);

					//Create all data
					var compressedBytes = ZlibStream.CompressBuffer(allData);
					int packetlength = compressedBytes.Length + dLength.Length;
					var dataToSend = isOver ? compressedBytes : allData;

					var compressed = new DataBuffer(_client);
					compressed.WriteVarInt(packetlength);
					compressed.WriteVarInt(dataLength);
					compressed.Write(dataToSend);

					Console.WriteLine();

					Console.WriteLine("Packet bigger than Threshold: " + isOver);
					Console.WriteLine("Packet info: ");

					Console.WriteLine("(Header) Packet Length: " + packetlength);
					Console.WriteLine("(Header) Data Length: " + dataLength);
					Console.WriteLine("Data Length " + dataToSend.Length);
					Console.WriteLine("Length difference: " + (packetlength - dataToSend.Length));

					Console.WriteLine();

					_client.AddToQuee(compressed.ExportWriter, quee);
				}
				else
				{
					WriteVarInt(allData.Length);
					var buffer = _bffr.ToArray();

					var data = new List<byte>();
					foreach (var i in buffer)
					{
						data.Add(i);
					}
					foreach (var i in allData)
					{
						data.Add(i);
					}
					_client.AddToQuee(data.ToArray(), quee);
				}
				_bffr.Clear();
			}
			catch (Exception ex)
			{
			//	ConsoleFunctions.WriteErrorLine("Failed to send a packet!\n" + ex);
				Globals.ClientManager.PacketError(_client, ex);
			}
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