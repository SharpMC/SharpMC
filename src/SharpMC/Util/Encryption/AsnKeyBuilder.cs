using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;

namespace SharpMC.Util.Encryption
{
    /// <summary>
    /// A class file to aide in working with ASN.1 encoded
    /// objects such as PKCS#8 PrivateKeyInfo messages and
    /// X.509 PublicKeyInfo messages. Useful for exporting
    /// a RSA or DSA key for use in Java and other non-XML
    /// encoded key aware languages.
    /// </summary>
    ///
    /// <remarks>
    /// Jeffrey Walton
    /// </remarks>
    internal sealed class AsnKeyBuilder
    {
        private readonly ILogger<AsnKeyBuilder> _log;

        public AsnKeyBuilder(ILogger<AsnKeyBuilder> log)
        {
            _log = log;
        }

        internal class AsnMessage
        {
            private readonly byte[] m_octets;
            private readonly string m_format;

            internal int Length => m_octets?.Length ?? 0;

            internal AsnMessage(byte[] octets, string format)
            {
                m_octets = octets;
                m_format = format;
            }

            internal byte[] GetBytes() => m_octets ?? new byte[] { };

            internal string GetFormat() => m_format;
        }

        internal class AsnType
        {
            // Constructors
            // No default - must specify tag and data

            public AsnType(byte tag, byte octet)
            {
                m_raw = false;
                m_tag = new[] {tag};
                m_octets = new[] {octet};
            }

            public AsnType(byte tag, byte[] octets)
            {
                m_raw = false;
                m_tag = new[] {tag};
                m_octets = octets;
            }

            public AsnType(byte tag, byte[] length, byte[] octets)
            {
                m_raw = true;
                m_tag = new[] {tag};
                m_length = length;
                m_octets = octets;
            }

            private bool m_raw;

            private bool Raw
            {
                get => m_raw;
                set => m_raw = value;
            }

            // Setters and Getters
            private byte[] m_tag;

            public byte[] Tag
            {
                get
                {
                    if (null == m_tag)
                        return EMPTY;
                    return m_tag;
                }
                // set { m_tag = value; }
            }

            private byte[] m_length;

            public byte[] Length
            {
                get
                {
                    if (null == m_length)
                        return EMPTY;
                    return m_length;
                }
                // set { m_length = value; }
            }

            private byte[] m_octets;

            public byte[] Octets
            {
                get
                {
                    if (null == m_octets)
                    {
                        return EMPTY;
                    }
                    return m_octets;
                }
                set => m_octets = value;
            }

            // Methods
            internal byte[] GetBytes()
            {
                // Created raw by user
                // return the bytes....
                if (true == m_raw)
                {
                    return Concatenate(
                        new[] {m_tag, m_length, m_octets}
                    );
                }

                SetLength();

                // Special case
                // Null does not use length
                if (0x05 == m_tag[0])
                {
                    return Concatenate(
                        new[] {m_tag, m_octets}
                    );
                }

                return Concatenate(
                    new[] {m_tag, m_length, m_octets}
                );
            }

            private void SetLength()
            {
                if (null == m_octets)
                {
                    m_length = ZERO;
                    return;
                }

                // Special case
                // Null does not use length
                if (0x05 == m_tag[0])
                {
                    m_length = EMPTY;
                    return;
                }

                byte[] length = null;

                // Length: 0 <= l < 0x80
                if (m_octets.Length < 0x80)
                {
                    length = new byte[1];
                    length[0] = (byte) m_octets.Length;
                }
                // 0x80 < length <= 0xFF
                else if (m_octets.Length <= 0xFF)
                {
                    length = new byte[2];
                    length[0] = 0x81;
                    length[1] = (byte) (m_octets.Length & 0xFF);
                }

                //
                // We should almost never see these...
                //

                // 0xFF < length <= 0xFFFF
                else if (m_octets.Length <= 0xFFFF)
                {
                    length = new byte[3];
                    length[0] = 0x82;
                    length[1] = (byte) ((m_octets.Length & 0xFF00) >> 8);
                    length[2] = (byte) (m_octets.Length & 0xFF);
                }

                // 0xFFFF < length <= 0xFFFFFF
                else if (m_octets.Length <= 0xFFFFFF)
                {
                    length = new byte[4];
                    length[0] = 0x83;
                    length[1] = (byte) ((m_octets.Length & 0xFF0000) >> 16);
                    length[2] = (byte) ((m_octets.Length & 0xFF00) >> 8);
                    length[3] = (byte) (m_octets.Length & 0xFF);
                }
                // 0xFFFFFF < length <= 0xFFFFFFFF
                else
                {
                    length = new byte[5];
                    length[0] = 0x84;
                    length[1] = (byte) ((m_octets.Length & 0xFF000000) >> 24);
                    length[2] = (byte) ((m_octets.Length & 0xFF0000) >> 16);
                    length[3] = (byte) ((m_octets.Length & 0xFF00) >> 8);
                    length[4] = (byte) (m_octets.Length & 0xFF);
                }

                m_length = length;
            }

            private byte[] Concatenate(byte[][] values)
            {
                // Nothing in, nothing out
                if (IsEmpty(values))
                    return new byte[] { };

                var length = 0;
                foreach (var b in values)
                {
                    if (null != b) length += b.Length;
                }

                var cated = new byte[length];

                var current = 0;
                foreach (var b in values)
                {
                    if (null != b)
                    {
                        Array.Copy(b, 0, cated, current, b.Length);
                        current += b.Length;
                    }
                }
                return cated;
            }
        }

        private static byte[] ZERO = {0};
        private static byte[] EMPTY = { };

        // PublicKeyInfo (X.509 compatible) message
        /// <summary>
        /// Returns the AsnMessage representing the X.509 PublicKeyInfo.
        /// </summary>
        /// <param name="publicKey">The DSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the
        /// X.509 PublicKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(DSAParameters)"/>
        /// <seealso cref="PrivateKeyToPKCS8(RSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(RSAParameters)"/>
        internal static AsnMessage PublicKeyToX509(DSAParameters publicKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != publicKey);

            /* *
            * SEQUENCE              // PrivateKeyInfo
            * +- SEQUENCE           // AlgorithmIdentifier
            * |  +- OID             // 1.2.840.10040.4.1
            * |  +- SEQUENCE        // DSS-Params (Optional Parameters)
            * |    +- INTEGER (P)
            * |    +- INTEGER (Q)
            * |    +- INTEGER (G)
            * +- BITSTRING          // PublicKey
            *    +- INTEGER(Y)      // DSAPublicKey Y
            * */

            // DSA Parameters
            var p = CreateIntegerPos(publicKey.P);
            var q = CreateIntegerPos(publicKey.Q);
            var g = CreateIntegerPos(publicKey.G);

            // Sequence - DSA-Params
            var dssParams = CreateSequence(new[] {p, q, g});

            // OID - packed 1.2.840.10040.4.1
            //   { 0x2A, 0x86, 0x48, 0xCE, 0x38, 0x04, 0x01 }
            var oid = CreateOid("1.2.840.10040.4.1");

            // Sequence
            var algorithmID = CreateSequence(new[] {oid, dssParams});

            // Public Key Y
            var y = CreateIntegerPos(publicKey.Y);
            var key = CreateBitString(y);

            // Sequence 'A'
            var publicKeyInfo =
                CreateSequence(new[] {algorithmID, key});

            return new AsnMessage(publicKeyInfo.GetBytes(), "X.509");
        }

        // PublicKeyInfo (X.509 compatible) message
        /// <summary>
        /// Returns the AsnMessage representing the X.509 PublicKeyInfo.
        /// </summary>
        /// <param name="publicKey">The RSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the
        /// X.509 PublicKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(DSAParameters)"/>
        /// <seealso cref="PrivateKeyToPKCS8(RSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(DSAParameters)"/>
        internal static AsnMessage PublicKeyToX509(RSAParameters publicKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != publicKey);

            /* *
            * SEQUENCE              // PrivateKeyInfo
            * +- SEQUENCE           // AlgorithmIdentifier
            *    +- OID             // 1.2.840.113549.1.1.1
            *    +- Null            // Optional Parameters
            * +- BITSTRING          // PrivateKey
            *    +- SEQUENCE        // RSAPrivateKey
            *       +- INTEGER(N)   // N
            *       +- INTEGER(E)   // E
            * */

            // OID - packed 1.2.840.113549.1.1.1
            //   { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 }
            var oid = CreateOid("1.2.840.113549.1.1.1");
            var algorithmID =
                CreateSequence(new[] {oid, CreateNull()});

            var n = CreateIntegerPos(publicKey.Modulus);
            var e = CreateIntegerPos(publicKey.Exponent);
            var key = CreateBitString(
                CreateSequence(new[] {n, e})
            );

            var publicKeyInfo =
                CreateSequence(new[] {algorithmID, key});

            return new AsnMessage(publicKeyInfo.GetBytes(), "X.509");
        }

        // PKCS #8, Section 6 (PrivateKeyInfo) message
        // !!!!!!!!!!!!!!! Unencrypted !!!!!!!!!!!!!!!
        /// <summary>
        /// Returns AsnMessage representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.
        /// </summary>
        /// <param name="privateKey">The DSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(RSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(DSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(RSAParameters)"/>
        internal static AsnMessage PrivateKeyToPKCS8(DSAParameters privateKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != privateKey);

            /* *
            * SEQUENCE              // PrivateKeyInfo
            * +- INTEGER(0)         // Version (v1998)
            * +- SEQUENCE           // AlgorithmIdentifier
            * |  +- OID             // 1.2.840.10040.4.1
            * |  +- SEQUENCE        // DSS-Params (Optional Parameters)
            * |    +- INTEGER (P)
            * |    +- INTEGER (Q)
            * |    +- INTEGER (G)
            * +- OCTETSTRING        // PrivateKey
            *    +- INTEGER(X)   // DSAPrivateKey X
            * */

            // Version - 0 (v1998)
            var version = CreateInteger(ZERO);

            // Domain Parameters
            var p = CreateIntegerPos(privateKey.P);
            var q = CreateIntegerPos(privateKey.Q);
            var g = CreateIntegerPos(privateKey.G);

            var dssParams = CreateSequence(new[] {p, q, g});

            // OID - packed 1.2.840.10040.4.1
            //   { 0x2A, 0x86, 0x48, 0xCE, 0x38, 0x04, 0x01 }
            var oid = CreateOid("1.2.840.10040.4.1");

            // AlgorithmIdentifier
            var algorithmID = CreateSequence(new[] {oid, dssParams});

            // Private Key X
            var x = CreateIntegerPos(privateKey.X);
            var key = CreateOctetString(x);

            // Sequence
            var privateKeyInfo =
                CreateSequence(new[] {version, algorithmID, key});

            return new AsnMessage(privateKeyInfo.GetBytes(), "PKCS#8");
        }

        // PKCS #8, Section 6 (PrivateKeyInfo) message
        // !!!!!!!!!!!!!!! Unencrypted !!!!!!!!!!!!!!!
        /// <summary>
        /// Returns AsnMessage representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.
        /// </summary>
        /// <param name="privateKey">The RSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(DSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(DSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(RSAParameters)"/>
        internal static AsnMessage PrivateKeyToPKCS8(RSAParameters privateKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != privateKey);

            /* *
            * SEQUENCE                  // PublicKeyInfo
            * +- INTEGER(0)             // Version - 0 (v1998)
            * +- SEQUENCE               // AlgorithmIdentifier
            *    +- OID                 // 1.2.840.113549.1.1.1
            *    +- NULL                // Optional Parameters
            * +- OCTETSTRING            // PrivateKey
            *    +- SEQUENCE            // RSAPrivateKey
            *       +- INTEGER(0)       // Version - 0 (v1998)
            *       +- INTEGER(N)
            *       +- INTEGER(E)
            *       +- INTEGER(D)
            *       +- INTEGER(P)
            *       +- INTEGER(Q)
            *       +- INTEGER(DP)
            *       +- INTEGER(DQ)
            *       +- INTEGER(Inv Q)
            * */

            var n = CreateIntegerPos(privateKey.Modulus);
            var e = CreateIntegerPos(privateKey.Exponent);
            var d = CreateIntegerPos(privateKey.D);
            var p = CreateIntegerPos(privateKey.P);
            var q = CreateIntegerPos(privateKey.Q);
            var dp = CreateIntegerPos(privateKey.DP);
            var dq = CreateIntegerPos(privateKey.DQ);
            var iq = CreateIntegerPos(privateKey.InverseQ);

            // Version - 0 (v1998)
            var version = CreateInteger(new byte[] {0});

            // octstring = OCTETSTRING(SEQUENCE(INTEGER(0)INTEGER(N)...))
            var key = CreateOctetString(
                CreateSequence(new[] {version, n, e, d, p, q, dp, dq, iq})
            );

            // OID - packed 1.2.840.113549.1.1.1
            //   { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 }
            var algorithmID = CreateSequence(new[] {CreateOid("1.2.840.113549.1.1.1"), CreateNull()}
            );

            // PrivateKeyInfo
            var privateKeyInfo =
                CreateSequence(new[] {version, algorithmID, key});

            return new AsnMessage(privateKeyInfo.GetBytes(), "PKCS#8");
        }

        public RSA DecodePublicKey(byte[] publicKeyBytes)
        {
            var ms = new MemoryStream(publicKeyBytes);
            var rd = new BinaryReader(ms);
            byte[] SeqOID = {0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00};
            var seq = new byte[15];

            try
            {
                byte byteValue;
                ushort shortValue;

                shortValue = rd.ReadUInt16();

                switch (shortValue)
                {
                    case 0x8130:
                        rd.ReadByte();
                        break;
                    case 0x8230:
                        rd.ReadInt16();
                        break;
                    default:
                        _log.LogWarning($"PublicKey Decode Returning null!");
                        return null;
                }

                seq = rd.ReadBytes(15);
                if (!Helpers.CompareBytearrays(seq, SeqOID)) return null;

                shortValue = rd.ReadUInt16();
                if (shortValue == 0x8103) rd.ReadByte();
                else if (shortValue == 0x8203)
                    rd.ReadInt16();
                else
                {
                    _log.LogWarning($"PublicKey Decode Returning null! (shortvalue 1)");
                    return null;
                }

                byteValue = rd.ReadByte();
                if (byteValue != 0x00)
                {
                    _log.LogWarning($"PublicKey Decode Returning null! (bytevalue)");
                    return null;
                }

                shortValue = rd.ReadUInt16();
                if (shortValue == 0x8130) rd.ReadByte();
                else if (shortValue == 0x8230)
                    rd.ReadInt16();
                else
                {
                    _log.LogWarning($"PublicKey Decode Returning null! (Shortvalue 2)");
                    return null;
                }


                var parms = new CspParameters();
                parms.Flags = CspProviderFlags.NoFlags;
                parms.KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant();
                parms.ProviderType =
                    Environment.OSVersion.Version.Major > 5 || Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1
                        ? 0x18
                        : 1;

                var rsa = RSA.Create();
                //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parms);
                var rsAparams = new RSAParameters();

                rsAparams.Modulus = rd.ReadBytes(Helpers.DecodeIntegerSize(rd));

                var traits = new RSAParameterTraits(rsAparams.Modulus.Length * 8);

                rsAparams.Modulus = Helpers.AlignBytes(rsAparams.Modulus, traits.size_Mod);
                rsAparams.Exponent = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_Exp);

                rsa.ImportParameters(rsAparams);
                return rsa;
            }
            catch (Exception e)
            {
                _log.LogWarning($"PublicKey Decode Exception: {e}");
                return null;
            }
            finally
            {
                rd.Close();
            }
        }

        /// <summary>
        /// <para>An ordered collection of one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType is null, an empty sequence (length 0)
        /// is returned.</para>
        /// </summary>
        /// <param name="value">An AsnType consisting of
        /// a single value to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded sequence.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        internal static AsnType CreateSequence(AsnType value)
        {
            // Should be at least 1...
            Debug.Assert(!IsEmpty(value));

            // One or more required
            if (IsEmpty(value))
            {
                throw new ArgumentException("A sequence requires at least one value.");
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType(0x30, value.GetBytes());
        }

        /// <summary>
        /// <para>An ordered collection of one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType is null, an
        /// empty sequence (length 0) is returned.</para>
        /// </summary>
        /// <param name="values">An array of AsnType consisting of
        /// the values in the collection to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded Set.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        internal static AsnType CreateSequence(AsnType[] values)
        {
            // Should be at least 1...
            Debug.Assert(!IsEmpty(values));

            // One or more required
            if (IsEmpty(values))
            {
                throw new ArgumentException("A sequence requires at least one value.");
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType(0x10 | 0x20, Concatenate(values));
        }

        /// <summary>
        /// <para>An ordered collection zero, one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType value is null,an
        /// empty sequence (length 0) is returned.</para>
        /// </summary>
        /// <param name="value">An AsnType consisting of
        /// a single value to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded sequence.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        internal static AsnType CreateSequenceOf(AsnType value)
        {
            // From the ASN.1 Mailing List
            if (IsEmpty(value))
            {
                return new AsnType(0x30, EMPTY);
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType(0x30, value.GetBytes());
        }

        /// <summary>
        /// <para>An ordered collection zero, one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType array is null or the array is 0 length,
        /// an empty sequence (length 0) is returned.</para>
        /// </summary>
        /// <param name="values">An AsnType consisting of
        /// the values in the collection to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded sequence.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        internal static AsnType CreateSequenceOf(AsnType[] values)
        {
            // From the ASN.1 Mailing List
            if (IsEmpty(values))
            {
                return new AsnType(0x30, EMPTY);
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType(0x30, Concatenate(values));
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// bit string is returned.</para>
        /// </summary>
        /// <param name="octets">A MSB (big endian) byte[] representing the
        /// bit string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(AsnType[])"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateBitString(byte[] octets)
        {
            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(octets, 0);
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.</para>
        /// <para>unusedBits is applied to the end of the bit string,
        /// not the start of the bit string. unusedBits must be less than 8
        /// (the size of an octet). Refer to ITU X.680, Section 32.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// bit string is returned.</para>
        /// </summary>
        /// <param name="octets">A MSB (big endian) byte[] representing the
        /// bit string to be encoded.</param>
        /// <param name="unusedBits">The number of unused trailing binary
        /// digits in the bit string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(AsnType[])"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateBitString(byte[] octets, uint unusedBits)
        {
            if (IsEmpty(octets))
            {
                // Empty octet string
                return new AsnType(0x03, EMPTY);
            }

            if (!(unusedBits < 8))
            {
                throw new ArgumentException("Unused bits must be less than 8.");
            }

            var b = Concatenate(new[] {(byte) unusedBits}, octets);
            // BitString: Tag 0x03 (3, Universal, Primitive)
            return new AsnType(0x03, b);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.
        /// If value is null, an empty (0 length) bit string is
        /// returned.
        /// </summary>
        /// <param name="value">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType[])"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateBitString(AsnType value)
        {
            if (IsEmpty(value))
            {
                return new AsnType(0x03, EMPTY);
            }

            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(value.GetBytes(), 0x00);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.
        /// If value is null, an empty (0 length) bit string is
        /// returned.
        /// </summary>
        /// <param name="values">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateBitString(AsnType[] values)
        {
            if (IsEmpty(values))
            {
                return new AsnType(0x03, EMPTY);
            }

            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(Concatenate(values), 0x00);
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// bit string is returned.</para>
        /// <para>If conversion fails, the bit string returned is a partial
        /// bit string. The partial bit string ends at the octet before the
        /// point of failure (it does not include the octet which could
        /// not be parsed, or subsequent octets).</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// bit string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateBitString(String value)
        {
            if (IsEmpty(value))
            {
                return CreateBitString(EMPTY);
            }

            // Any unused bits?
            var lstrlen = value.Length;
            var unusedBits = 8 - lstrlen % 8;
            if (8 == unusedBits)
            {
                unusedBits = 0;
            }

            for (var i = 0; i < unusedBits; i++)
            {
                value += "0";
            }

            // Determine number of octets
            var loctlen = (lstrlen + 7) / 8;

            var octets = new List<byte>();
            for (var i = 0; i < loctlen; i++)
            {
                var s = value.Substring(i * 8, 8);
                byte b = 0x00;

                try
                {
                    b = Convert.ToByte(s, 2);
                }

                catch (FormatException /*e*/)
                {
                    unusedBits = 0;
                    break;
                }
                catch (OverflowException /*e*/)
                {
                    unusedBits = 0;
                    break;
                }

                octets.Add(b);
            }

            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(octets.ToArray(), (uint) unusedBits);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more octets. Returns
        /// the ASN.1 encoded octet string. If octets is null or length
        /// is 0, an empty (0 length) octet string is returned.
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// octet string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateOctetString(byte[] value)
        {
            if (IsEmpty(value))
            {
                // Empty octet string
                return new AsnType(0x04, EMPTY);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return new AsnType(0x04, value);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more octets. Returns
        /// the byte[] representing an ASN.1 encoded octet string.
        /// If octets is null or length is 0, an empty (0 length)
        /// o ctet string is returned.
        /// </summary>
        /// <param name="value">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateOctetString(AsnType value)
        {
            if (IsEmpty(value))
            {
                // Empty octet string
                return new AsnType(0x04, 0x00);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return new AsnType(0x04, value.GetBytes());
        }

        /// <summary>
        /// An ordered sequence of zero, one or more octets. Returns
        /// the byte[] representing an ASN.1 encoded octet string.
        /// If octets is null or length is 0, an empty (0 length)
        /// o ctet string is returned.
        /// </summary>
        /// <param name="values">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(String)"/>
        internal static AsnType CreateOctetString(AsnType[] values)
        {
            if (IsEmpty(values))
            {
                // Empty octet string
                return new AsnType(0x04, 0x00);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return new AsnType(0x04, Concatenate(values));
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded octet string.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// octet string is returned.</para>
        /// <para>If conversion fails, the bit string returned is a partial
        /// bit string. The partial octet string ends at the octet before the
        /// point of failure (it does not include the octet which could
        /// not be parsed, or subsequent octets).</para>
        /// </summary>
        /// <param name="value">A string representing the
        /// octet string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        internal static AsnType CreateOctetString(String value)
        {
            if (IsEmpty(value))
            {
                return CreateOctetString(EMPTY);
            }

            // Determine number of octets
            var len = (value.Length + 255) / 256;

            var octets = new List<byte>();
            for (var i = 0; i < len; i++)
            {
                var s = value.Substring(i * 2, 2);
                byte b = 0x00;

                try
                {
                    b = Convert.ToByte(s, 16);
                }
                catch (FormatException /*e*/)
                {
                    break;
                }
                catch (OverflowException /*e*/)
                {
                    break;
                }

                octets.Add(b);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return CreateOctetString(octets.ToArray());
        }

        /// <summary>
        /// <para>Returns the AsnType representing a ASN.1 encoded
        /// integer. The octets pass through this method are not modified.</para>
        /// <para>If octets is null or zero length, the method returns an
        /// AsnType equivalent to CreateInteger(byte[]{0})..</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// integer to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded integer.</returns>
        /// <example>
        /// ASN.1 encoded 0:
        /// <code>CreateInteger(null)</code>
        /// <code>CreateInteger(new byte[]{0x00})</code>
        /// <code>CreateInteger(new byte[]{0x00, 0x00})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded 1:
        /// <code>CreateInteger(new byte[]{0x01})</code>
        /// </example>
        /// <seealso cref="CreateIntegerPos"/>
        /// <seealso cref="CreateIntegerNeg"/>
        internal static AsnType CreateInteger(byte[] value)
        {
            // Is it better to add a '0', or silently
            //   drop the Integer? Dropping integers
            //   is probably not te best choice...
            if (IsEmpty(value))
            {
                return CreateInteger(ZERO);
            }

            return new AsnType(0x02, value);
        }

        /// <summary>
        /// <para>Returns the AsnType representing a positive ASN.1 encoded
        /// integer. If the high bit of most significant byte is set,
        /// the method prepends a 0x00 to octets before assigning the
        /// value to ensure the resulting integer is interpreted as
        /// positive in the application.</para>
        /// <para>If octets is null or zero length, the method returns an
        /// AsnType equivalent to CreateInteger(byte[]{0})..</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// integer to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded positive integer.</returns>
        /// <example>
        /// ASN.1 encoded 0:
        /// <code>CreateIntegerPos(null)</code>
        /// <code>CreateIntegerPos(new byte[]{0x00})</code>
        /// <code>CreateIntegerPos(new byte[]{0x00, 0x00})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded 1:
        /// <code>CreateInteger(new byte[]{0x01})</code>
        /// </example>
        /// <seealso cref="CreateInteger"/>
        /// <seealso cref="CreateIntegerNeg"/>
        internal static AsnType CreateIntegerPos(byte[] value)
        {
            byte[] i = null, d = Duplicate(value);

            if (IsEmpty(d))
            {
                d = ZERO;
            }

            // Mediate the 2's compliment representation.
            // If the first byte has its high bit set, we will
            // add the additional byte of 0x00
            if (d.Length > 0 && d[0] > 0x7F)
            {
                i = new byte[d.Length + 1];
                i[0] = 0x00;
                Array.Copy(d, 0, i, 1, value.Length);
            }
            else
            {
                i = d;
            }

            // Integer: Tag 0x02 (2, Universal, Primitive)
            return CreateInteger(i);
        }

        /// <summary>
        /// <para>Returns the negative ASN.1 encoded integer. If the high
        /// bit of most significant byte is set, the integer is already
        /// considered negative.</para>
        /// <para>If the high bit of most significant byte
        /// is <bold>not</bold> set, the integer will be 2's complimented
        /// to form a negative integer.</para>
        /// <para>If octets is null or zero length, the method returns an
        /// AsnType equivalent to CreateInteger(byte[]{0})..</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// integer to be encoded.</param>
        /// <returns>Returns the negative ASN.1 encoded integer.</returns>
        /// <example>
        /// ASN.1 encoded 0:
        /// <code>CreateIntegerNeg(null)</code>
        /// <code>CreateIntegerNeg(new byte[]{0x00})</code>
        /// <code>CreateIntegerNeg(new byte[]{0x00, 0x00})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -1 (2's compliment 0xFF):
        /// <code>CreateIntegerNeg(new byte[]{0x01})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -2 (2's compliment 0xFE):
        /// <code>CreateIntegerNeg(new byte[]{0x02})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -1:
        /// <code>CreateIntegerNeg(new byte[]{0xFF})</code>
        /// <code>CreateIntegerNeg(new byte[]{0xFF,0xFF})</code>
        /// Note: already negative since the high bit is set.</example>
        /// <example>
        /// ASN.1 encoded -255 (2's compliment 0xFF, 0x01):
        /// <code>CreateIntegerNeg(new byte[]{0x00,0xFF})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -255 (2's compliment 0xFF, 0xFF, 0x01):
        /// <code>CreateIntegerNeg(new byte[]{0x00,0x00,0xFF})</code>
        /// </example>
        /// <seealso cref="CreateInteger"/>
        /// <seealso cref="CreateIntegerPos"/>
        internal static AsnType CreateIntegerNeg(byte[] value)
        {
            // Is it better to add a '0', or silently
            //   drop the Integer? Dropping integers
            //   is probably not te best choice...
            if (IsEmpty(value))
            {
                return CreateInteger(ZERO);
            }

            // No Trimming
            // The byte[] may be that way for a reason
            if (IsZero(value))
            {
                return CreateInteger(value);
            }

            //
            // At this point, we know we have at least 1 octet
            //

            // Is this integer already negative?
            if (value[0] >= 0x80)
                // Pass through with no modifications
            {
                return CreateInteger(value);
            }

            // No need to Duplicate - Compliment2s
            // performs the action
            var c = Compliment2s(value);

            return CreateInteger(c);
        }

        /// <summary>
        /// Returns the AsnType representing an ASN.1 encoded null.
        /// </summary>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded null.</returns>
        internal static AsnType CreateNull()
        {
            return new AsnType(0x05, new byte[] {0x00});
        }

        /// <summary>
        /// Removes leading 0x00 octets from the byte[] octets. This
        /// method may return an empty byte array (0 length).
        /// </summary>
        /// <param name="octets">An array of octets to trim.</param>
        /// <returns>A byte[] with leading 0x00 octets removed.</returns>
        internal static byte[] TrimStart(byte[] octets)
        {
            if (IsEmpty(octets) || IsZero(octets))
            {
                return new byte[] { };
            }

            var d = Duplicate(octets);

            // RenderPosition of the first non-zero value
            var pos = 0;
            foreach (var b in d)
            {
                if (0 != b)
                {
                    break;
                }
                pos++;
            }

            // Nothing to trim
            if (pos == d.Length)
            {
                return octets;
            }

            // Allocate trimmed array
            var t = new byte[d.Length - pos];

            // Copy
            Array.Copy(d, pos, t, 0, t.Length);

            return t;
        }

        /// <summary>
        /// Removes trailing 0x00 octets from the byte[] octets. This
        /// method may return an empty byte array (0 length).
        /// </summary>
        /// <param name="octets">An array of octets to trim.</param>
        /// <returns>A byte[] with trailing 0x00 octets removed.</returns>
        internal static byte[] TrimEnd(byte[] octets)
        {
            if (IsEmpty(octets) || IsZero(octets))
            {
                return EMPTY;
            }

            var d = Duplicate(octets);

            Array.Reverse(d);

            d = TrimStart(d);

            Array.Reverse(d);

            return d;
        }

        /// <summary>
        /// Returns the AsnType representing an ASN.1 encoded OID.
        /// If conversion fails, the result is a partial conversion
        /// up to the point of failure. If the oid string is null or
        /// not well formed, an empty byte[] is returned.
        /// </summary>
        /// <param name="value">The string representing the object
        /// identifier to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded object identifier.</returns>
        /// <example>The following assigns the encoded AsnType
        /// for a RSA key to oid:
        /// <code>AsnType oid = CreateOid("1.2.840.113549.1.1.1")</code>
        /// </example>
        /// <seealso cref="CreateOid(byte[])"/>
        internal static AsnType CreateOid(String value)
        {
            // Punt?
            if (IsEmpty(value))
                return null;

            var tokens = value.Split(new[] {' ', '.'});

            // Punt?
            if (IsEmpty(tokens))
                return null;

            // Parsing/Manipulation of the arc value
            UInt64 a = 0;

            // One or more strings are available
            var arcs = new List<UInt64>();

            foreach (var t in tokens)
            {
                // No empty or ill-formed strings...
                if (t.Length == 0)
                {
                    break;
                }

                try
                {
                    a = Convert.ToUInt64(t, CultureInfo.InvariantCulture);
                }
                catch (FormatException /*e*/)
                {
                    break;
                }
                catch (OverflowException /*e*/)
                {
                    break;
                }

                arcs.Add(a);
            }

            // Punt?
            if (0 == arcs.Count)
                return null;

            // Octets to be returned to caller
            var octets = new List<byte>();

            // Guard the case of a small list
            // The list has at least 1 item...    
            if (arcs.Count >= 1)
            {
                a = arcs[0] * 40;
            }
            if (arcs.Count >= 2)
            {
                a += arcs[1];
            }
            octets.Add((byte) a);

            // Add remaining arcs (subidentifiers)
            for (var i = 2; i < arcs.Count; i++)
            {
                // Scratch list builder for this arc
                var temp = new List<byte>();

                // The current arc (subidentifier)
                var arc = arcs[i];

                // Build the arc (subidentifier) byte array
                // The array is built in reverse (LSB to MSB).
                do
                {
                    // Each entry is formed from the low 7 bits (0x7F).
                    // Set high bit of all entries (0x80) per X.680. We
                    // will unset the high bit of the final byte later.
                    temp.Add((byte) (0x80 | (arc & 0x7F)));
                    arc >>= 7;
                } while (0 != arc);

                // Grab resulting array. Because of the do/while,
                // there is at least one value in the array.
                var t = temp.ToArray();

                // Unset high bit of byte t[0]
                // t[0] will be LSB after the array is reversed.
                t[0] = (byte) (0x7F & t[0]);

                // MSB first...
                Array.Reverse(t);

                // Add to the resulting array
                foreach (var b in t)
                {
                    octets.Add(b);
                }
            }

            return CreateOid(octets.ToArray());
        }

        /// <summary>
        /// Returns the AsnType representing an ASN.1 encoded OID.
        /// If conversion fails, the result is a partial conversion
        /// (up to the point of failure). If octets is null, an
        /// empty byte[] is returned.
        /// </summary>
        /// <param name="value">The packed byte[] representing the object
        /// identifier to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded object identifier.</returns>
        /// <example>The following assigns the encoded AsnType for a RSA
        /// key to oid:
        /// <code>// Packed 1.2.840.113549.1.1.1
        /// byte[] rsa = new byte[] { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 };
        /// AsnType = CreateOid(rsa)</code>
        /// </example>
        /// <seealso cref="CreateOid(String)"/>
        internal static AsnType CreateOid(byte[] value)
        {
            // Punt...
            if (IsEmpty(value))
            {
                return null;
            }

            // OID: Tag 0x06 (6, Universal, Primitive)
            return new AsnType(0x06, value);
        }

        private static byte[] Compliment1s(byte[] value)
        {
            if (IsEmpty(value))
            {
                return EMPTY;
            }

            // Make a copy of octet array
            var c = Duplicate(value);

            for (var i = c.Length - 1; i >= 0; i--)
            {
                // Compliment
                c[i] = (byte) ~c[i];
            }

            return c;
        }

        private static byte[] Compliment2s(byte[] value)
        {
            if (IsEmpty(value))
            {
                return EMPTY;
            }

            // 2s Compliment of 0 is 0
            if (IsZero(value))
            {
                return Duplicate(value);
            }

            // Make a copy of octet array
            var d = Duplicate(value);

            var carry = 1;
            for (var i = d.Length - 1; i >= 0; i--)
            {
                // Compliment
                d[i] = (byte) ~d[i];

                // Add
                var j = d[i] + carry;

                // Write Back
                d[i] = (byte) (j & 0xFF);

                // Determine Next Carry
                if (0x100 == (j & 0x100))
                {
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
            }

            // Carry Array (we may need to carry out of 'd'
            byte[] c = null;
            if (1 == carry)
            {
                c = new byte[d.Length + 1];

                // Sign Extend....
                c[0] = 0xFF;

                Array.Copy(d, 0, c, 1, d.Length);
            }
            else
            {
                c = d;
            }

            return c;
        }

        private static byte[] Concatenate(AsnType[] values)
        {
            // Nothing in, nothing out
            if (IsEmpty(values))
                return new byte[] { };

            var length = 0;
            foreach (var t in values)
            {
                if (null != t)
                {
                    length += t.GetBytes().Length;
                }
            }

            var cated = new byte[length];

            var current = 0;
            foreach (var t in values)
            {
                if (null != t)
                {
                    var b = t.GetBytes();

                    Array.Copy(b, 0, cated, current, b.Length);
                    current += b.Length;
                }
            }

            return cated;
        }

        private static byte[] Concatenate(byte[] first, byte[] second)
        {
            return Concatenate(new[] {first, second});
        }

        private static byte[] Concatenate(byte[][] values)
        {
            // Nothing in, nothing out
            if (IsEmpty(values))
                return new byte[] { };

            var length = 0;
            foreach (var b in values)
            {
                if (null != b)
                {
                    length += b.Length;
                }
            }

            var cated = new byte[length];

            var current = 0;
            foreach (var b in values)
            {
                if (null != b)
                {
                    Array.Copy(b, 0, cated, current, b.Length);
                    current += b.Length;
                }
            }

            return cated;
        }

        private static byte[] Duplicate(byte[] b)
        {
            if (IsEmpty(b))
            {
                return EMPTY;
            }

            var d = new byte[b.Length];
            Array.Copy(b, d, b.Length);

            return d;
        }

        private static bool IsZero(byte[] octets)
        {
            if (IsEmpty(octets))
            {
                return false;
            }

            var allZeros = true;
            for (var i = 0; i < octets.Length; i++)
            {
                if (0 != octets[i])
                {
                    allZeros = false;
                    break;
                }
            }
            return allZeros;
        }

        private static bool IsEmpty(byte[] octets)
        {
            if (null == octets || 0 == octets.Length)
            {
                return true;
            }

            return false;
        }

        private static bool IsEmpty(String s)
        {
            if (null == s || 0 == s.Length)
            {
                return true;
            }

            return false;
        }

        private static bool IsEmpty(String[] strings)
        {
            if (null == strings || 0 == strings.Length)
                return true;

            return false;
        }

        private static bool IsEmpty(AsnType value)
        {
            if (null == value)
            {
                return true;
            }

            return false;
        }

        private static bool IsEmpty(AsnType[] values)
        {
            if (null == values || 0 == values.Length)
                return true;

            return false;
        }

        private static bool IsEmpty(byte[][] arrays)
        {
            if (null == arrays || 0 == arrays.Length)
                return true;

            return false;
        }

        private class Helpers
        {
            public static bool CompareBytearrays(byte[] a, byte[] b)
            {
                if (a.Length != b.Length)
                    return false;
                var i = 0;
                foreach (var c in a)
                {
                    if (c != b[i])
                        return false;
                    i++;
                }
                return true;
            }

            public static byte[] AlignBytes(byte[] inputBytes, int alignSize)
            {
                var inputBytesSize = inputBytes.Length;

                if (alignSize != -1 && inputBytesSize < alignSize)
                {
                    var buf = new byte[alignSize];
                    for (var i = 0; i < inputBytesSize; ++i)
                    {
                        buf[i + (alignSize - inputBytesSize)] = inputBytes[i];
                    }
                    return buf;
                }
                else
                {
                    return inputBytes;
                }
            }

            public static int DecodeIntegerSize(BinaryReader rd)
            {
                byte byteValue;
                int count;

                byteValue = rd.ReadByte();
                if (byteValue != 0x02) return 0;

                byteValue = rd.ReadByte();
                if (byteValue == 0x81)
                {
                    count = rd.ReadByte();
                }
                else if (byteValue == 0x82)
                {
                    var hi = rd.ReadByte();
                    var lo = rd.ReadByte();
                    count = BitConverter.ToUInt16(new[] {lo, hi}, 0);
                }
                else
                {
                    count = byteValue;
                }

                while (rd.ReadByte() == 0x00)
                {
                    count -= 1;
                }
                rd.BaseStream.Seek(-1, SeekOrigin.Current);

                return count;
            }
        }

        private class RSAParameterTraits
        {
            public RSAParameterTraits(int modulusLengthInBits)
            {
                var assumedLength = -1;
                var logbase = Math.Log(modulusLengthInBits, 2);
                if (logbase == (int) logbase)
                {
                    assumedLength = modulusLengthInBits;
                }
                else
                {
                    assumedLength = (int) (logbase + 1.0);
                    assumedLength = (int) Math.Pow(2, assumedLength);
                    Debug.Assert(false);
                }

                switch (assumedLength)
                {
                    case 512:
                        size_Mod = 0x40;
                        size_Exp = -1;
                        size_D = 0x40;
                        size_P = 0x20;
                        size_Q = 0x20;
                        size_DP = 0x20;
                        size_DQ = 0x20;
                        size_InvQ = 0x20;
                        break;
                    case 1024:
                        size_Mod = 0x80;
                        size_Exp = -1;
                        size_D = 0x80;
                        size_P = 0x40;
                        size_Q = 0x40;
                        size_DP = 0x40;
                        size_DQ = 0x40;
                        size_InvQ = 0x40;
                        break;
                    case 2048:
                        size_Mod = 0x100;
                        size_Exp = -1;
                        size_D = 0x100;
                        size_P = 0x80;
                        size_Q = 0x80;
                        size_DP = 0x80;
                        size_DQ = 0x80;
                        size_InvQ = 0x80;
                        break;
                    case 4096:
                        size_Mod = 0x200;
                        size_Exp = -1;
                        size_D = 0x200;
                        size_P = 0x100;
                        size_Q = 0x100;
                        size_DP = 0x100;
                        size_DQ = 0x100;
                        size_InvQ = 0x100;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }

            public int size_Mod = -1;
            public int size_Exp = -1;
            public int size_D = -1;
            public int size_P = -1;
            public int size_Q = -1;
            public int size_DP = -1;
            public int size_DQ = -1;
            public int size_InvQ = -1;
        }
    }
}