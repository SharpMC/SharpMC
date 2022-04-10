namespace SharpMC.API.Entities
{
    public interface IMonster : IMob
    {
        bool Aware { get; set; }

        void Damage(double amount);
    }
}