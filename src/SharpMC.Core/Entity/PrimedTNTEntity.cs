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
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Entity
{
	public class PrimedTNTEntity : Entity
	{
		public int Fuse = 30;

		public PrimedTNTEntity(Level level)
			: base(50, level)
		{
			Height = 0.98;
			Width = 0.98;
			Length = 0.98;
		}

		public override void DespawnEntity()
		{
			foreach (var i in Level.OnlinePlayers)
			{
				var spawnedBy = i.Wrapper;

				new DestroyEntities(spawnedBy)
				{
					EntityIds = new[] {EntityId}
				}.Write();
			}
			Level.RemoveEntity(this);
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
					Type = ObjectType.ActivatedTnt,
					Data = 0
				}.Write();
			}
		}

		public override void OnTick()
		{
			Fuse--;

			if (Fuse <= 0)
			{
				DespawnEntity();

				foreach (var player in Level.OnlinePlayers)
				{
					new Particle(player.Wrapper)
					{
						X = (float) KnownPosition.X,
						Y = (float) KnownPosition.Y,
						Z = (float) KnownPosition.Z,
						ParticleId = 2,
						ParticleCount = 1,
						Data = new int[0]
					}.Write();
					new SoundEffect(player.Wrapper) {X = (int) KnownPosition.X, Y = (int) KnownPosition.Y, Z = (int) KnownPosition.Z}
						.Write();
				}
				new Explosion(Level, new Vector3(KnownPosition.X, KnownPosition.Y, KnownPosition.Z), 5f).Explode();
			}
		}
	}
}