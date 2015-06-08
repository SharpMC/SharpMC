using System;
using System.Text;
using JetBrains.Annotations;

namespace fNbt {
    /// <summary> A tag containing a single string. String is stored in UTF-8 encoding. </summary>
    public sealed class NbtString : NbtTag {
        /// <summary> Type of this tag (String). </summary>
        public override NbtTagType TagType {
            get {
                return NbtTagType.String;
            }
        }

        /// <summary> Value/payload of this tag (a single string). May not be <c>null</c>. </summary>
        [NotNull]
        public string Value {
            get {
                return stringVal;
            }
            set {
                if( value == null ) {
                    throw new ArgumentNullException( "value" );
                }
                stringVal = value;
            }
        }

        [NotNull]
        string stringVal;


        /// <summary> Creates an unnamed NbtString tag with the default value (empty string). </summary>
        public NbtString() {}


        /// <summary> Creates an unnamed NbtString tag with the given value. </summary>
        /// <param name="value"> String value to assign to this tag. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public NbtString( [NotNull] string value )
            : this( null, value ) {}


        /// <summary> Creates an NbtString tag with the given name and value. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> String value to assign to this tag. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public NbtString( [CanBeNull] string tagName, [NotNull] string value ) {
            if( value == null )
                throw new ArgumentNullException( "value" );
            Name = tagName;
            Value = value;
        }


        #region Reading / Writing

        internal override bool ReadTag( NbtBinaryReader readStream ) {
            if( readStream.Selector != null && !readStream.Selector( this ) ) {
                readStream.SkipString();
                return false;
            }
            Value = readStream.ReadString();
            return true;
        }


        internal override void SkipTag( NbtBinaryReader readStream ) {
            readStream.SkipString();
        }


        internal override void WriteTag( NbtBinaryWriter writeStream, bool writeName ) {
            writeStream.Write( NbtTagType.String );
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


        /// <summary> Returns a String that represents the current NbtString object.
        /// Format: TAG_String("Name"): Value </summary>
        /// <returns> A String that represents the current NbtString object. </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            PrettyPrint( sb, null, 0 );
            return sb.ToString();
        }


        internal override void PrettyPrint( StringBuilder sb, string indentString, int indentLevel ) {
            for( int i = 0; i < indentLevel; i++ ) {
                sb.Append( indentString );
            }
            sb.Append( "TAG_String" );
            if( !String.IsNullOrEmpty( Name ) ) {
                sb.AppendFormat( "(\"{0}\")", Name );
            }
            sb.Append( ": \"" );
            sb.Append( Value );
            sb.Append( '"' );
        }
    }
}