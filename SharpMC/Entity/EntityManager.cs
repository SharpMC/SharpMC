namespace SharpMC.Entity
{
	public class EntityManager
	{
		private static int LastEntityId = 0;

		public static int GetEntityId()
		{
			LastEntityId++;
			return LastEntityId;
		}
	}
}
