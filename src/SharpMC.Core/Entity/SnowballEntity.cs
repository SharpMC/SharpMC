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

using SharpMC.Core.Enums;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Entity
{
	public class SnowballEntity : Projectile
	{
		public SnowballEntity(Player shooter, Level level) : base(shooter, 61, level)
		{
			Width = 0.25;
			Length = 0.25;
			Height = 0.25;

			Gravity = 0.03;
			Drag = 0.01;
		}

		public override void SpawnEntity()
		{
			Level.AddEntity(this);
			foreach (var i in Level.OnlinePlayers)
			{
				var spawnedBy = i.Wrapper;
				new SpawnObject(spawnedBy)
				{
					EntityId = EntityId,
					X = KnownPosition.X,
					Y = KnownPosition.Y,
					Z = KnownPosition.Z,
					Type = ObjectType.Snowball,
					Data = Shooter.EntityId,
					Pitch = (byte) KnownPosition.Pitch,
					Yaw = (byte) KnownPosition.Yaw
				}.Write();
			}
		}
	}
}