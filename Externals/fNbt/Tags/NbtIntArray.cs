using System;
using System.Text;
using JetBrains.Annotations;

namespace fNbt {
    /// <summary> A tag containing an array of signed 32-bit integers. </summary>
    public sealed class NbtIntArray : NbtTag {
        /// <summary> Type of this tag (ByteArray). </summary>
        public override NbtTagType TagType {
            get {
                return NbtTagType.IntArray;
            }
        }


        /// <summary> Value/payload of this tag (an array of signed 32-bit integers). May not be <c>null</c>. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        [NotNull]
        public int[] Value {
            get {
                return ints;
            }
            set {
                if( value == null ) {
                    throw new ArgumentNullException( "value" );
                }
                ints = value;
            }
        }

        [NotNull]
        int[] ints;


        /// <summary> Creates an unnamed NbtIntArray tag, containing an empty array of ints. </summary>
        public NbtIntArray()
            : this( null, new int[0] ) {}


        /// <summary> Creates an unnamed NbtIntArray tag, containing the given array of ints. </summary>
        /// <param name="value"> Int array to assign to this tag's Value. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public NbtIntArray( [NotNull] int[] value )
            : this( null, value ) {}


        /// <summary> Creates an NbtIntArray tag with the given name, containing an empty array of ints. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        public NbtIntArray( [CanBeNull] string tagName )
            : this( tagName, new int[0] ) {}


        /// <summary> Creates an NbtIntArray tag with the given name, containing the given array of ints. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> Int array to assign to this tag's Value. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public NbtIntArray( [CanBeNull] string tagName, [NotNull] int[] value ) {
            if( value == null )
                throw new ArgumentNullException( "value" );
            Name = tagName;
            Value = (int[])value.Clone();
        }


        /// <summary> Gets or sets an integer at the given index. </summary>
        /// <param name="tagIndex"> The zero-based index of the element to get or set. </param>
        /// <returns> The integer at the specified index. </returns>
        /// <exception cref="IndexOutOfRangeException"> <paramref name="tagIndex"/> is outside the array bounds. </exception>
        public new int this[ int tagIndex ] {
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
                throw new NbtFormatException( "Negative length given in TAG_Int_Array" );
            }

            if( readStream.Selector != null && !readStream.Selector( this ) ) {
                readStream.Skip( length * sizeof( int ) );
                return false;
            }

            Value = new int[length];
            for( int i = 0; i < length; i++ ) {
                Value[i] = readStream.ReadInt32();
            }
            return true;
        }


        internal override void SkipTag( NbtBinaryReader readStream ) {
            int length = readStream.ReadInt32();
            if( length < 0 ) {
                throw new NbtFormatException( "Negative length given in TAG_Int_Array" );
            }
            readStream.Skip( length * sizeof( int ) );
        }


        internal override void WriteTag( NbtBinaryWriter writeStream, bool writeName ) {
            writeStream.Write( NbtTagType.IntArray );
            if( writeName ) {
                if( Name == null )
                    throw new NbtFormatException( "Name is null" );
                writeStream.Write( Name );
            }
            WriteData( writeStream );
        }


        internal override void WriteData( NbtBinaryWriter writeStream ) {
            writeStream.Write( Value.Length );
            for( int i = 0; i < Value.Length; i++ ) {
                writeStream.Write( Value[i] );
            }
        }


        /// <summary> Returns a String that represents the current NbtIntArray object.
        /// Format: TAG_Int_Array("Name"): [N ints] </summary>
        /// <returns> A String that represents the current NbtIntArray object. </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            PrettyPrint( sb, null, 0 );
            return sb.ToString();
        }


        internal override void PrettyPrint( StringBuilder sb, string indentString, int indentLevel ) {
            for( int i = 0; i < indentLevel; i++ ) {
                sb.Append( indentString );
            }
            sb.Append( "TAG_Int_Array" );
            if( !String.IsNullOrEmpty( Name ) ) {
                sb.AppendFormat( "(\"{0}\")", Name );
            }
            sb.AppendFormat( ": [{0} ints]", ints.Length );
        }
    }
}