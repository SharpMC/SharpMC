using System;
using System.Numerics;
using System.Timers;
using SharpMC.Core.Utils;
using SharpMC.Util;
using SharpMC.World;
using PlayerLocation = SharpMC.Util.PlayerLocation;

namespace SharpMC.Entities
{
	public class Entity
	{
        public bool IsSpawned { get; set; }

		protected Entity(Level level)
		{
			Level = level;
			KnownPosition = new PlayerLocation();

			EntityId = EntityManager.EntityIdUndefined;
		}

		public virtual void SpawnEntity()
		{
			Level.AddEntity(this);

			IsSpawned = true;
		}

		public virtual void SpawnToPlayers(Player[] players)
		{

		}

		public virtual void DespawnFromPlayers(Player[] players)
		{
			
		}

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
            KnownPosition = new PlayerLocation(0, 0, 0);
            HealthManager = new HealthManager(this);
        }

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
        public int Age { get; private set; }

        public virtual void OnTick()
        {
            Age++;
        }

        public virtual void DespawnEntity()
        {
            Level.RemoveEntity(this);
        }

        public byte GetDirection()
        {
            return DirectionByRotationFlat(KnownPosition.Yaw);
        }

        public static byte DirectionByRotationFlat(float yaw)
        {
            var direction = (byte)((int)Math.Floor(yaw * 4F / 360F + 0.5D) & 0x03);
            switch (direction)
            {
                case 0:
                    return 1; // West
                case 1:
                    return 2; // North
                case 2:
                    return 3; // East
                case 3:
                    return 0; // South 
            }
            return 0;
        }

        public BoundingBox GetBoundingBox()
        {
            var pos = KnownPosition;
            var halfWidth = Width / 2;

            return new BoundingBox(
                Vectors.Create(pos.X - halfWidth, pos.Y, pos.Z - halfWidth),
                Vectors.Create(pos.X + halfWidth, pos.Y + Height, pos.Z + halfWidth)
            );
        }
    }
}