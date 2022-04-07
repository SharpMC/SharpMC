using System;
using SharpMC.Network.Util;
using SharpMC.Util;

namespace SharpMC.World
{
	public class ChunkSection
	{
		//4096

		public const int Depth = 16;
		public const int Width = 16;
		public const int Height = 16;

		private const int TotalBlocks = Depth*Width*Height;


		public VariableValueArray Types { get; set; }
		public NibbleArray SkyLight { get; set; } 
		public NibbleArray BlockLight { get; set; }

		public ChunkSection()
		{
			SkyLight = new NibbleArray(TotalBlocks);
			BlockLight = new NibbleArray(TotalBlocks);

			Types = new VariableValueArray(13, TotalBlocks);
			for (var i = 0; i < TotalBlocks; i++)
			{
				Types[i] = 0;
				SkyLight[i] = 0xff;
			}

			AirBlocks = TotalBlocks;
		}

		private int AirBlocks { get; set; }
		public bool IsAllAir => AirBlocks == TotalBlocks;

		public void ReCalculateAirBlocks()
		{
			var airCounter = 0;
			for (var i = 0; i < TotalBlocks; i++)
			{
				if (Types[i] >> 4 == 0) airCounter++;
			}
			AirBlocks = airCounter;
		}

		private static int GetIndex(int x, int y, int z)
		{
			if (x < 0 || z < 0 || y < 0 || x >= Width || z >= Depth || y >= Height)
				throw new IndexOutOfRangeException("Coords (x=" + x + ",y=" + y + ",z=" + z + ") invalid");

			return (y << 8) | (z << 4) | x;
			//return (z * Height + y) * Width + x;
		}

		public short GetBlockId(int x, int y, int z)
		{
			return (short) (Types[GetIndex(x, y, z)] >> 4);
		}

		public byte GetBlockData(int x, int y, int z)
		{
			return (byte) (Types[GetIndex(x, y, z)] & 15);
		}

		public void SetBlockId(int x, int y, int z, short id)
		{
			var index = GetIndex(x, y, z);
			var data = Types[index];

			var type = data >> 4;
			var metadata = data & 15;

			if (type == 0 && id > 0)
			{
				AirBlocks--;
			}
			else if (type > 0 && id == 0)
			{
				AirBlocks++;
			}

			Types[index] = id << 4 | (metadata & 15);
		}

		public void SetBlockData(int x, int y, int z, byte meta)
		{
			var index = GetIndex(x, y, z);
			var data = Types[index];
			var type = data >> 4;
			//int metadata = data & 15;

			Types[index] = type << 4 | (meta & 15);
		}



		public void WriteTo(MinecraftStream stream, bool writeSkylight = true)
		{
			var types = Types.Backing;

			stream.WriteByte(13); //Bits Per Block
			stream.WriteVarInt(0); //Palette Length
			
			stream.WriteVarInt(types.Length);
			for (var i = 0; i < types.Length; i++)
			{
				stream.WriteLong(types[i]);
			}

			stream.Write(BlockLight.Data);

			if (writeSkylight)
				stream.Write(SkyLight.Data);
		}
	}
}
