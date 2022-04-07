using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.Util;
using SharpMC.World;

namespace SharpMC.Core.Entity
{
	public class PrimedTNTEntity : Entities.Entity
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
			foreach (var i in Level.GetOnlinePlayers)
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
			foreach (var i in Level.GetOnlinePlayers)
			{
				var spawnedBy = i.Wrapper;
				new SpawnObject(spawnedBy)
				{
					EntityId = EntityId,
					X = KnownPosition.X,
					Y = KnownPosition.Y,
					Z = KnownPosition.Z,
					Type = ObjectType.ActivatedTnt,
					Info = 0
				}.Write();
			}
		}

		public override void OnTick()
		{
			Fuse--;

			if (Fuse <= 0)
			{
				DespawnEntity();

				foreach (var player in Level.GetOnlinePlayers)
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
				new Explosion(Level, 
					Vectors.Create(KnownPosition.X, KnownPosition.Y, KnownPosition.Z), 5f).Explode();
			}
		}
	}
}