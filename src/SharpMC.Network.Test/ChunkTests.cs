using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpMC.Chunky;
using SharpMC.Chunky.Palette;
using SharpMC.Data;
using SharpMC.Network.Util;
using Xunit;
using static SharpMC.Blocks.KnownBlocks;
using static SharpMC.Network.Test.DataBunch3;
using static SharpMC.Network.Test.TestHelper;

namespace SharpMC.Network.Test
{
    public class ChunkTests
    {
        [Theory]
        [InlineData(1, 17417)]
        [InlineData(2, 17429)]
        [InlineData(3, 18699)]
        public void ShouldReadChunk(int idx, int size)
        {
            var input = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, input.Length);
            
            
        }

        [Theory]
        [InlineData(1, 17417)]
        [InlineData(2, 17429)]
        [InlineData(3, 18699)]
        public void ShouldWriteChunk(int idx, int size)
        {
            var expected = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, expected.Length);
            
            
        }
    }
}