using System;
using System.IO;
using NUnit.Framework;

namespace fNbt.Test {
    [TestFixture]
    public sealed class NbtReaderTests {
        [Test]
        public void PrintBigFileUncompressed() {
            using( FileStream fs = File.OpenRead( "TestFiles/bigtest.nbt" ) ) {
                NbtReader reader = new NbtReader( fs );
                while( reader.ReadToFollowing() ) {
                    Console.Write( "@" + reader.TagStartOffset + " " );
                    Console.WriteLine( reader.ToString( true ) );
                }
                Assert.AreEqual( reader.RootName, "Level" );
            }
        }


        [Test]
        public void NestedListTest() {
            NbtCompound root = new NbtCompound( "root" ) {
                new NbtList( "OuterList" ) {
                    new NbtList {
                        new NbtByte()
                    },
                    new NbtList {
                        new NbtShort()
                    },
                    new NbtList {
                        new NbtInt()
                    }
                }
            };
            byte[] testData = new NbtFile( root ).SaveToBuffer( NbtCompression.None );
            using( MemoryStream ms = new MemoryStream( testData ) ) {
                NbtReader reader = new NbtReader( ms );
                while( reader.ReadToFollowing() ) {
                    Console.WriteLine( reader.ToString( true ) );
                }
            }
        }


        static Stream MakeTest() {
            NbtCompound root = new NbtCompound( "root" ) {
                new NbtInt( "first" ),
                new NbtInt( "second" ),
                new NbtCompound( "third-comp" ) {
                    new NbtInt( "inComp1" ),
                    new NbtInt( "inComp2" ),
                    new NbtInt( "inComp3" )
                },
                new NbtList( "fourth-list" ) {
                    new NbtList {
                        new NbtCompound {
                            new NbtCompound( "inList1" )
                        }
                    },
                    new NbtList {
                        new NbtCompound {
                            new NbtCompound( "inList2" )
                        }
                    },
                    new NbtList {
                        new NbtCompound {
                            new NbtCompound( "inList3" )
                        }
                    }
                },
                new NbtInt( "fifth" )
            };
            byte[] testData = new NbtFile( root ).SaveToBuffer( NbtCompression.None );
            return new MemoryStream( testData );
        }


        [Test]
        public void PropertiesTest() {
            NbtReader reader = new NbtReader( MakeTest() );
            Assert.AreEqual( reader.Depth, 0 );
            Assert.AreEqual( reader.TagsRead, 0 );

            Assert.IsTrue( reader.ReadToFollowing() );
            Assert.AreEqual( reader.TagName, "root" );
            Assert.AreEqual( reader.TagType, NbtTagType.Compound );
            Assert.AreEqual( reader.ListType, NbtTagType.Unknown );
            Assert.IsFalse( reader.HasValue );
            Assert.IsTrue( reader.IsCompound );
            Assert.IsFalse( reader.IsList );
            Assert.IsFalse( reader.IsListElement );
            Assert.IsFalse( reader.HasLength );
            Assert.AreEqual( reader.ListIndex, 0 );
            Assert.AreEqual( reader.Depth, 1 );
            Assert.AreEqual( reader.ParentName, null );
            Assert.AreEqual( reader.ParentTagType, NbtTagType.Unknown );
            Assert.AreEqual( reader.ParentTagLength, 0 );
            Assert.AreEqual( reader.TagLength, 0 );
            Assert.AreEqual( reader.TagsRead, 1 );

            Assert.IsTrue( reader.ReadToFollowing() );
            Assert.AreEqual( reader.TagName, "first" );
            Assert.AreEqual( reader.TagType, NbtTagType.Int );
            Assert.AreEqual( reader.ListType, NbtTagType.Unknown );
            Assert.IsTrue( reader.HasValue );
            Assert.IsFalse( reader.IsCompound );
            Assert.IsFalse( reader.IsList );
            Assert.IsFalse( reader.IsListElement );
            Assert.IsFalse( reader.HasLength );
            Assert.AreEqual( reader.ListIndex, 0 );
            Assert.AreEqual( reader.Depth, 2 );
            Assert.AreEqual( reader.ParentName, "root" );
            Assert.AreEqual( reader.ParentTagType, NbtTagType.Compound );
            Assert.AreEqual( reader.ParentTagLength, 0 );
            Assert.AreEqual( reader.TagLength, 0 );
            Assert.AreEqual( reader.TagsRead, 2 );

            Assert.IsTrue( reader.ReadToFollowing( "fourth-list" ) );
            Assert.AreEqual( reader.TagName, "fourth-list" );
            Assert.AreEqual( reader.TagType, NbtTagType.List );
            Assert.AreEqual( reader.ListType, NbtTagType.List );
            Assert.IsFalse( reader.HasValue );
            Assert.IsFalse( reader.IsCompound );
            Assert.IsTrue( reader.IsList );
            Assert.IsFalse( reader.IsListElement );
            Assert.IsTrue( reader.HasLength );
            Assert.AreEqual( reader.ListIndex, 0 );
            Assert.AreEqual( reader.Depth, 2 );
            Assert.AreEqual( reader.ParentName, "root" );
            Assert.AreEqual( reader.ParentTagType, NbtTagType.Compound );
            Assert.AreEqual( reader.ParentTagLength, 0 );
            Assert.AreEqual( reader.TagLength, 3 );
            Assert.AreEqual( reader.TagsRead, 8 );

            Assert.IsTrue( reader.ReadToFollowing() ); // first list element, itself a list
            Assert.AreEqual( reader.TagName, null );
            Assert.AreEqual( reader.TagType, NbtTagType.List );
            Assert.AreEqual( reader.ListType, NbtTagType.Compound );
            Assert.IsFalse( reader.HasValue );
            Assert.IsFalse( reader.IsCompound );
            Assert.IsTrue( reader.IsList );
            Assert.IsTrue( reader.IsListElement );
            Assert.IsTrue( reader.HasLength );
            Assert.AreEqual( reader.ListIndex, 0 );
            Assert.AreEqual( reader.Depth, 3 );
            Assert.AreEqual( reader.ParentName, "fourth-list" );
            Assert.AreEqual( reader.ParentTagType, NbtTagType.List );
            Assert.AreEqual( reader.ParentTagLength, 3 );
            Assert.AreEqual( reader.TagLength, 1 );
            Assert.AreEqual( reader.TagsRead, 9 );

            Assert.IsTrue( reader.ReadToFollowing() ); // first nested list element, compound
            Assert.AreEqual( reader.TagName, null );
            Assert.AreEqual( reader.TagType, NbtTagType.Compound );
            Assert.AreEqual( reader.ListType, NbtTagType.Unknown );
            Assert.IsFalse( reader.HasValue );
            Assert.IsTrue( reader.IsCompound );
            Assert.IsFalse( reader.IsList );
            Assert.IsTrue( reader.IsListElement );
            Assert.IsFalse( reader.HasLength );
            Assert.AreEqual( reader.ListIndex, 0 );
            Assert.AreEqual( reader.Depth, 4 );
            Assert.AreEqual( reader.ParentName, null );
            Assert.AreEqual( reader.ParentTagType, NbtTagType.List );
            Assert.AreEqual( reader.ParentTagLength, 1 );
            Assert.AreEqual( reader.TagLength, 0 );
            Assert.AreEqual( reader.TagsRead, 10 );
        }


        [Test]
        public void ReadToSiblingTest() {
            NbtReader reader = new NbtReader( MakeTest() );
            Assert.IsTrue( reader.ReadToFollowing() );
            Assert.AreEqual( reader.TagName, "root" );
            Assert.IsTrue( reader.ReadToFollowing() );
            Assert.AreEqual( reader.TagName, "first" );
            Assert.IsTrue( reader.ReadToNextSibling( "third-comp" ) );
            Assert.AreEqual( reader.TagName, "third-comp" );
            Assert.IsTrue( reader.ReadToNextSibling() );
            Assert.AreEqual( reader.TagName, "fourth-list" );
            Assert.IsTrue( reader.ReadToNextSibling() );
            Assert.AreEqual( reader.TagName, "fifth" );
            Assert.IsFalse( reader.ReadToNextSibling() );
        }


        [Test]
        public void ReadToDescendantTest() {
            NbtReader reader = new NbtReader( MakeTest() );
            Assert.IsTrue( reader.ReadToDescendant( "third-comp" ) );
            Assert.AreEqual( reader.TagName, "third-comp" );
            Assert.IsTrue( reader.ReadToDescendant( "inComp2" ) );
            Assert.AreEqual( reader.TagName, "inComp2" );
            Assert.IsFalse( reader.ReadToDescendant( "derp" ) );
            Assert.AreEqual( reader.TagName, "inComp3" );
            reader.ReadToFollowing(); // at fourth-list
            Assert.IsTrue( reader.ReadToDescendant( "inList2" ) );
            Assert.AreEqual( reader.TagName, "inList2" );
        }


        [Test]
        public void SkipTest() {
            NbtReader reader = new NbtReader( MakeTest() );
            reader.ReadToFollowing(); // at root
            reader.ReadToFollowing(); // at first
            reader.ReadToFollowing(); // at second
            reader.ReadToFollowing(); // at third-comp
            reader.ReadToFollowing(); // at inComp1
            Assert.AreEqual( reader.TagName, "inComp1" );
            Assert.AreEqual( reader.Skip(), 2 );
            Assert.AreEqual( reader.TagName, "fourth-list" );
            Assert.AreEqual( reader.Skip(), 10 );
            Assert.IsFalse( reader.ReadToFollowing() );
        }


        [Test]
        public void ReadAsTagTest() {
            NbtReader reader = new NbtReader( MakeTest() );
            Console.WriteLine( reader.ReadAsTag().ToString() );
        }


        [Test]
        public void ReadValueTest() {
            NbtCompound root = new NbtCompound( "root" ) {
                new NbtByte( "byte", 1 ),
                new NbtShort( "short", 2 ),
                new NbtInt( "int", 3 ),
                new NbtLong( "long", 4 ),
                new NbtFloat( "float", 5 ),
                new NbtDouble( "double", 6 ),
                new NbtByteArray( "byteArray", new byte[] { 10, 11, 12 } ),
                new NbtIntArray( "intArray", new[] { 20, 21, 22 } ),
                new NbtString( "string", "23" )
            };
            byte[] testData = new NbtFile( root ).SaveToBuffer( NbtCompression.None );
            NbtReader reader = new NbtReader( new MemoryStream( testData ) );

            Assert.IsTrue( reader.ReadToFollowing() ); // root

            Assert.IsTrue( reader.ReadToFollowing() ); // byte
            Assert.AreEqual( reader.ReadValue(), 1 );
            Assert.IsTrue( reader.ReadToFollowing() ); // short
            Assert.AreEqual( reader.ReadValue(), 2 );
            Assert.IsTrue( reader.ReadToFollowing() ); // int
            Assert.AreEqual( reader.ReadValue(), 3 );
            Assert.IsTrue( reader.ReadToFollowing() ); // long
            Assert.AreEqual( reader.ReadValue(), 4 );
            Assert.IsTrue( reader.ReadToFollowing() ); // float
            Assert.AreEqual( reader.ReadValue(), 5f );
            Assert.IsTrue( reader.ReadToFollowing() ); // double
            Assert.AreEqual( reader.ReadValue(), 6d );
            Assert.IsTrue( reader.ReadToFollowing() ); // byteArray
            CollectionAssert.AreEqual( (byte[])reader.ReadValue(), new byte[] { 10, 11, 12 } );
            Assert.IsTrue( reader.ReadToFollowing() ); // intArray
            CollectionAssert.AreEqual( (int[])reader.ReadValue(), new[] { 20, 21, 22 } );
            Assert.IsTrue( reader.ReadToFollowing() ); // string
            Assert.AreEqual( reader.ReadValue(), "23" );
        }
    }
}