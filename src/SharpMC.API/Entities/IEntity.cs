namespace SharpMC.API.Entities
{
    public interface IEntity
    {
        int EntityId { get; set; }

        bool IsSpawned { get; set; }

        void SpawnToPlayers(IPlayer[] players);

        void DespawnFromPlayers(IPlayer[] players);
    }
}