using System;
using System.Text;
using JetBrains.Annotations;

namespace fNbt {
    /// <summary> A tag containing an array of bytes. </summary>
    public sealed class NbtByteArray : NbtTag {
        /// <summary> Type of this tag (ByteArray). </summary>
        public override NbtTagType TagType {
            get {
                return NbtTagType.ByteArray;
            }
        }


        /// <summary> Value/payload of this tag (an array of bytes). May not be <c>null</c>. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        [NotNull]
        public byte[] Value {
            get {
                return bytes;
            }
            set {
                if( value == null ) {
                    throw new ArgumentNullException( "value" );
                }
                bytes = value;
            }
        }

        [NotNull]
        byte[] bytes;


        /// <summary> Creates an unnamed NbtByte tag, containing an empty array of bytes. </summary>
        public NbtByteArray()
            : this( null, new byte[0] ) {}


        /// <summary> Creates an unnamed NbtByte tag, containing the given array of bytes. </summary>
        /// <param name="value"> Byte array to assign to this tag's Value. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public NbtByteArray( [NotNull] byte[] value )
            : this( null, value ) {}


        /// <summary> Creates an NbtByte tag with the given name, containing an empty array of bytes. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        public NbtByteArray( [CanBeNull] string tagName )
            : this( tagName, new byte[0] ) {}


        /// <summary> Creates an NbtByte tag with the given name, containing the given array of bytes. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> Byte array to assign to this tag's Value. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public NbtByteArray( [CanBeNull] string tagName, [NotNull] byte[] value ) {
            if( value == null )
                throw new ArgumentNullException( "value" );
            Name = tagName;
            Value = (byte[])value.Clone();
        }


        /// <summary> Gets or sets a byte at the given index. </summary>
        /// <param name="tagIndex"> The zero-based index of the element to get or set. </param>
        /// <returns> The byte at the specified index. </returns>
        /// <exception cref="IndexOutOfRangeException"> <paramref name="tagIndex"/> is outside the array bounds. </exception>
        public new byte this[ int tagIndex ] {
            get {
                return Value[tagIndex];
            }
            set {
                Value[tagIndex] = value;
            }
        }


        internal override bool ReadTag( NbtBinaryReader readStream ) {
            int length = readStream.ReadInt32();
            if( length < 0 ) {
                throw new NbtFormatException( "Negative length given in TAG_Byte_Array" );
            }

            if( readStream.Selector != null && !readStream.Selector( this ) ) {
                readStream.Skip( length );
                return false;
            }
            Value = readStream.ReadBytes( length );
            return true;
        }


        internal override void SkipTag( NbtBinaryReader readStream ) {
            int length = readStream.ReadInt32();
            if( length < 0 ) {
                throw new NbtFormatException( "Negative length given in TAG_Byte_Array" );
            }
            readStream.Skip( length );
        }


        internal override void WriteTag( NbtBinaryWriter writeStream, bool writeName ) {
            writeStream.Write( NbtTagType.ByteArray );
            if( writeName ) {
                if( Name == null )
                    throw new NbtFormatException( "Name is null" );
                writeStream.Write( Name );
            }
            WriteData( writeStream );
        }


        internal override void WriteData( NbtBinaryWriter writeStream ) {
            writeStream.Write( Value.Length );
            writeStream.Write( Value, 0, Value.Length );
        }


        /// <summary> Returns a String that represents the current NbtByteArray object.
        /// Format: TAG_Byte_Array("Name"): [N bytes] </summary>
        /// <returns> A String that represents the current NbtByteArray object. </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            PrettyPrint( sb, null, 0 );
            return sb.ToString();
        }


        internal override void PrettyPrint( StringBuilder sb, string indentString, int indentLevel ) {
            for( int i = 0; i < indentLevel; i++ ) {
                sb.Append( indentString );
            }
            sb.Append( "TAG_Byte_Array" );
            if( !String.IsNullOrEmpty( Name ) ) {
                sb.AppendFormat( "(\"{0}\")", Name );
            }
            sb.AppendFormat( ": [{0} bytes]", bytes.Length );
        }
    }
}