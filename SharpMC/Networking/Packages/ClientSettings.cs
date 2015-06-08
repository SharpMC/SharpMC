// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class ClientSettings : Package<ClientSettings>
	{
		public ClientSettings(ClientWrapper client) : base(client)
		{
			ReadId = 0x15;
		}

		public ClientSettings(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x15;
		}

		public override void Read()
		{
			var Locale = Buffer.ReadString();
			var ViewDistance = (byte) Buffer.ReadByte();
			var ChatFlags = (byte) Buffer.ReadByte();
			var ChatColours = Buffer.ReadBool();
			var SkinParts = (byte) Buffer.ReadByte();

			Client.Player.Locale = Locale;
			Client.Player.ViewDistance = ViewDistance;
			Client.Player.ChatColours = ChatColours;
			Client.Player.ChatFlags = ChatFlags;
			Client.Player.SkinParts = SkinParts;
		}
	}
}