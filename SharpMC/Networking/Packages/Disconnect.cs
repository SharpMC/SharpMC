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

using SharpMC.Entity;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Networking.Packages
{
	public class Disconnect : Package<Disconnect>
	{
		public string Reason = "";

		public Disconnect(ClientWrapper client) : base(client)
		{
			SendId = 0x40;
		}

		public Disconnect(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x40;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString("{ \"text\": \"" + Reason + "\" }");
				Buffer.FlushData();
			}
		}

		public static void Broadcast(string reason, Level level = null, bool self = true, Player source = null)
		{
			if (level == null)
			{
				foreach (var lvl in Globals.LevelManager.GetLevels())
				{
					foreach (var i in lvl.OnlinePlayers)
					{
						if (!self && i == source)
						{
							continue;
						}
						//Client = i.Wrapper;
						//Buffer = new DataBuffer(i.Wrapper);
						//_stream = i.Wrapper.TCPClient.GetStream();
						//Write();
						new Disconnect(i.Wrapper, new DataBuffer(i.Wrapper)) { Reason = reason }.Write();
					}
				}

				foreach (var i in Globals.LevelManager.MainLevel.OnlinePlayers)
				{
					if (!self && i == source)
					{
						continue;
					}
					//Client = i.Wrapper;
					//Buffer = new DataBuffer(i.Wrapper);
					//_stream = i.Wrapper.TCPClient.GetStream();
					//Write();
					new Disconnect(i.Wrapper, new DataBuffer(i.Wrapper)) { Reason = reason }.Write();
				}
			}
			else
			{
				foreach (var i in level.OnlinePlayers)
				{
					if (!self && i == source)
					{
						continue;
					}
					//Client = i.Wrapper;
					//Buffer = new DataBuffer(i.Wrapper);
					//_stream = i.Wrapper.TCPClient.GetStream();
					//Write();
					new Disconnect(i.Wrapper, new DataBuffer(i.Wrapper)) { Reason = reason }.Write();
				}
			}
		}
	}
}