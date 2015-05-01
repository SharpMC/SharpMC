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
