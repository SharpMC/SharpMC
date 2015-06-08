using System;
using System.IO;
using NUnit.Framework;

namespace fNbt.Test {
    [TestFixture]
    public sealed class ListTests {
        const string TempDir = "TestTemp";

        [SetUp]
        public void ListTestsSetup() {
            Directory.CreateDirectory( TempDir );
        }


        [Test]
        public void InitializingListFromCollection() {
            // auto-detecting list type
            Assert.DoesNotThrow( () => new NbtList( "Test1", new NbtTag[] {
                new NbtInt( 1 ),
                new NbtInt( 2 ),
                new NbtInt( 3 )
            } ) );

            Assert.AreEqual( new NbtList( "Test1", new NbtTag[] {
                new NbtInt( 1 ),
                new NbtInt( 2 ),
                new NbtInt( 3 )
            } ).ListType, NbtTagType.Int );

            // correct explicitly-given list type
            Assert.DoesNotThrow( () => new NbtList( "Test2", new NbtTag[] {
                new NbtInt( 1 ),
                new NbtInt( 2 ),
                new NbtInt( 3 )
            }, NbtTagType.Int ) );

            // wrong explicitly-given list type
            Assert.Throws<ArgumentException>( () => new NbtList( "Test3", new NbtTag[] {
                new NbtInt( 1 ),
                new NbtInt( 2 ),
                new NbtInt( 3 )
            }, NbtTagType.Float ) );

            // auto-detecting mixed list given
            Assert.Throws<ArgumentException>( () => new NbtList( "Test4", new NbtTag[] {
                new NbtFloat( 1 ),
                new NbtByte( 2 ),
                new NbtInt( 3 )
            } ) );
        }


        [Test]
        public void ManipulatingList() {
            var sameTags = new NbtTag[] {
                new NbtInt( 0 ),
                new NbtInt( 1 ),
                new NbtInt( 2 )
            };

            NbtList list = new NbtList( "Test1", sameTags );

            // testing enumerator
            int j = 0;
            foreach( NbtTag tag in list ) {
                Assert.AreEqual( tag, sameTags[j++] );
            }

            // adding an item of correct type
            list.Add( new NbtInt( 3 ) );
            list.Insert( 3, new NbtInt( 4 ) );

            // adding an item of wrong type
            Assert.Throws<ArgumentException>( () => list.Add( new NbtString() ) );
            Assert.Throws<ArgumentException>( () => list.Insert( 3, new NbtString() ) );

            // testing array contents
            for( int i = 0; i < sameTags.Length; i++ ) {
                Assert.AreSame( sameTags[i], list[i] );
                Assert.AreEqual( ( (NbtInt)list[i] ).Value, i );
            }

            // test removal
            Assert.IsFalse( list.Remove( new NbtInt( 5 ) ) );
            Assert.IsTrue( list.Remove( sameTags[0] ) );
            list.RemoveAt( 0 );
            Assert.Throws<ArgumentOutOfRangeException>( () => list.RemoveAt( 10 ) );
        }


        [Test]
        public void ChangingListTagType() {
            var list = new NbtList();

            // changing type of an empty list
            Assert.DoesNotThrow( () => list.ListType = NbtTagType.Unknown );

            list.Add( new NbtInt() );

            // setting correct type for a non-empty list
            Assert.DoesNotThrow( () => list.ListType = NbtTagType.Int );

            // changing list type to an incorrect type
            Assert.Throws<ArgumentException>( () => list.ListType = NbtTagType.Short );
        }


        [Test]
        public void SerializingWithoutListType() {
            NbtCompound root = new NbtCompound( "root" ) { new NbtList( "list" ) };
            NbtFile file = new NbtFile( root );

            using( MemoryStream ms = new MemoryStream() ) {
                // list should throw NbtFormatException, because its ListType is Unknown
                Assert.Throws<NbtFormatException>( () => file.SaveToStream( ms, NbtCompression.None ) );
            }
        }


        [Test]
        public void Serializing() {
            string fileName = Path.Combine( TempDir, "NbtListType.nbt" );
            const NbtTagType expectedListType = NbtTagType.Int;
            const int elements = 10;

            // construct nbt file
            NbtFile writtenFile = new NbtFile( new NbtCompound( "ListTypeTest" ) );
            NbtList writtenList = new NbtList( "Entities", null, expectedListType );
            for( int i = 0; i < elements; i++ ) {
                writtenList.Add( new NbtInt( i ) );
            }
            writtenFile.RootTag.Add( writtenList );

            // test saving
            writtenFile.SaveToFile( fileName, NbtCompression.GZip );

            // test loading
            NbtFile readFile = new NbtFile( fileName );

            // check contents of loaded file
            Assert.NotNull( readFile.RootTag );
            Assert.IsInstanceOf<NbtList>( readFile.RootTag["Entities"] );
            NbtList readList = (NbtList)readFile.RootTag["Entities"];
            Assert.AreEqual( readList.ListType, writtenList.ListType );
            Assert.AreEqual( readList.Count, writtenList.Count );

            // check .ToArray
            CollectionAssert.AreEquivalent( readList, readList.ToArray() );
            CollectionAssert.AreEquivalent( readList, readList.ToArray<NbtInt>() );

            // check contents of loaded list
            for( int i = 0; i < elements; i++ ) {
                Assert.AreEqual( readList.Get<NbtInt>( i ).Value, writtenList.Get<NbtInt>( i ).Value );
            }
        }


        [TearDown]
        public void ListTestsTearDown() {
            if( Directory.Exists( TempDir ) ) {
                foreach( var file in Directory.GetFiles( TempDir ) ) {
                    File.Delete( file );
                }
                Directory.Delete( TempDir );
            }
        }
    }
}