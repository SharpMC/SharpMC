namespace SharpMC.World.API.Noises
{
    public interface INoiseGenerator
    {
        void SetScale(double scale);

        double Noise(double x, double y, double z, double frequency, double amplitude);

        double Noise(double x, double z, double frequency, double amplitude);
    }
}