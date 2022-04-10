namespace SharpMC.API.Entities
{
    public interface ITameable : IAnimal, INameable
    {
        bool IsTamed { get; }

        IPlayer Owner { get; set; }
    }
}