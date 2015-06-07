#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Utils
{
	using fNbt;
	using fNbt.Serialization;

	/// <summary>
	///     Represents an array of 4-bit values.
	/// </summary>
	public class NibbleArray : INbtSerializable
	{
		public NibbleArray()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NibbleArray"/> class. 
		/// Creates a new nibble array with the given number of nibbles.
		/// </summary>
		/// <param name="length">
		/// The length.
		/// </param>
		public NibbleArray(int length)
		{
			this.Data = new byte[length / 2];
		}

		/// <summary>
		///     The data in the nibble array. Each byte contains
		///     two nibbles, stored in big-endian.
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		///     Gets the current number of nibbles in this array.
		/// </summary>
		[NbtIgnore]
		public int Length
		{
			get
			{
				return this.Data.Length * 2;
			}
		}

		/// <summary>
		/// Gets or sets a nibble at the given index.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="byte"/>.
		/// </returns>
		[NbtIgnore]
		public byte this[int index]
		{
			get
			{
				return (byte)(this.Data[index / 2] >> (index % 2 * 4) & 0xF);
			}

			set
			{
				value &= 0xF;
				this.Data[index / 2] &= (byte)(0xF << ((index + 1) % 2 * 4));
				this.Data[index / 2] |= (byte)(value << (index % 2 * 4));
			}
		}

		public NbtTag Serialize(string tagName)
		{
			return new NbtByteArray(tagName, this.Data);
		}

		public void Deserialize(NbtTag value)
		{
			this.Data = value.ByteArrayValue;
		}
	}
}