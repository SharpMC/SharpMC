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
using System;
using System.Timers;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Entity
{
	public class Entity
	{
		public Level Level { get; set; }

		public int EntityTypeId { get; private set; }
		public int EntityId { get; set; }
		public byte Dimension { get; set; }
		public DateTime LastUpdatedTime { get; set; }
		public PlayerLocation KnownPosition { get; set; }
		public Vector3 Velocity { get; set; }

		public HealthManager HealthManager { get; private set; }

		public double Height { get; set; }
		public double Width { get; set; }
		public double Length { get; set; }
		public double Drag { get; set; }
		public double Gravity { get; set; }
		public int Data { get; set; }

		public readonly Timer TickTimer = new Timer();

		public Entity(int entityTypeId, Level level)
		{
			Height = 1;
			Width = 1;
			Length = 1;
			Drag = 0;
			Gravity = 0;

			EntityId = EntityManager.GetEntityId();
			Level = level;
			EntityTypeId = entityTypeId;
			KnownPosition = new PlayerLocation(0,0,0);
			HealthManager = new HealthManager(this);
		}

		public virtual void OnTick()
		{
		}
	}
}
