using System;
using System.Text;

namespace fNbt {
    /// <summary> A tag containing a single signed 64-bit integer. </summary>
    public sealed class NbtLong : NbtTag {
        /// <summary> Type of this tag (Long). </summary>
        public override NbtTagType TagType {
            get {
                return NbtTagType.Long;
            }
        }

        /// <summary> Value/payload of this tag (a single signed 64-bit integer). </summary>
        public long Value { get; set; }


        /// <summary> Creates an unnamed NbtLong tag with the default value of 0. </summary>
        public NbtLong() {}


        /// <summary> Creates an unnamed NbtLong tag with the given value. </summary>
        /// <param name="value"> Value to assign to this tag. </param>
        public NbtLong( long value )
            : this( null, value ) {}


        /// <summary> Creates an NbtLong tag with the given name and the default value of 0. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        public NbtLong( string tagName )
            : this( tagName, 0 ) {}


        /// <summary> Creates an NbtLong tag with the given name and value. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> Value to assign to this tag. </param>
        public NbtLong( string tagName, long value ) {
            Name = tagName;
            Value = value;
        }


        #region Reading / Writing

        internal override bool ReadTag( NbtBinaryReader readStream ) {
            if( readStream.Selector != null && !readStream.Selector( this ) ) {
                readStream.ReadInt64();
                return false;
            }
            Value = readStream.ReadInt64();
            return true;
        }


        internal override void SkipTag( NbtBinaryReader readStream ) {
            readStream.ReadInt64();
        }


        internal override void WriteTag( NbtBinaryWriter writeStream, bool writeName ) {
            writeStream.Write( NbtTagType.Long );
            if( writeName ) {
                if( Name == null )
                    throw new NbtFormatException( "Name is null" );
                writeStream.Write( Name );
            }
            writeStream.Write( Value );
        }


        internal override void WriteData( NbtBinaryWriter writeStream ) {
            writeStream.Write( Value );
        }

        #endregion


        /// <summary> Returns a String that represents the current NbtLong object.
        /// Format: TAG_Long("Name"): Value </summary>
        /// <returns> A String that represents the current NbtLong object. </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            PrettyPrint( sb, null, 0 );
            return sb.ToString();
        }


        internal override void PrettyPrint( StringBuilder sb, string indentString, int indentLevel ) {
            for( int i = 0; i < indentLevel; i++ ) {
                sb.Append( indentString );
            }
            sb.Append( "TAG_Long" );
            if( !String.IsNullOrEmpty( Name ) ) {
                sb.AppendFormat( "(\"{0}\")", Name );
            }
            sb.Append( ": " );
            sb.Append( Value );
        }
    }
}