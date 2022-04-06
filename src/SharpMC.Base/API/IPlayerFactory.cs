namespace SharpMC.API
{
	public interface IPlayerFactory
	{
		Player CreatePlayer(MCNetConnection connection, string username);
	}
}
