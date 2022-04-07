using SharpMC.Enums;
using SharpMC.World;

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

			Ttl = 9999999;
			ObjectType = ObjectType.Snowball;
		}

		/*public override void SpawnEntity()
		{
			Ttl = 9999999;
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
		}*/
	}
}