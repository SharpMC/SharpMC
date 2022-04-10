namespace SharpMC.API.Entities
{
    public interface IAnimal : IMob, IAgeable
    {
        int LoveModeTicks { get; set; }

        bool Breed { get; set; }
    }
}