using NUnit.Framework;

namespace fNbt.Test {
    [TestFixture]
    public sealed class TagSelectorTests {
        [Test]
        public void SkippingTagsOnFileLoad() {
            NbtFile loadedFile = new NbtFile();
            loadedFile.LoadFromFile( "TestFiles/bigtest.nbt",
                                     NbtCompression.None,
                                     tag => tag.Name != "nested compound test" );
            Assert.IsFalse( loadedFile.RootTag.Contains( "nested compound test" ) );
            Assert.IsTrue( loadedFile.RootTag.Contains( "listTest (long)" ) );

            loadedFile.LoadFromFile( "TestFiles/bigtest.nbt",
                                     NbtCompression.None,
                                     tag => tag.TagType != NbtTagType.Float || tag.Parent.Name != "Level" );
            Assert.IsFalse( loadedFile.RootTag.Contains( "floatTest" ) );
            Assert.AreEqual( loadedFile.RootTag["nested compound test"]["ham"]["value"].FloatValue, 0.75 );

            loadedFile.LoadFromFile( "TestFiles/bigtest.nbt",
                                     NbtCompression.None,
                                     tag => tag.Name != "listTest (long)" );
            Assert.IsFalse( loadedFile.RootTag.Contains( "listTest (long)" ) );
            Assert.IsTrue( loadedFile.RootTag.Contains( "byteTest" ) );
        }
    }
}