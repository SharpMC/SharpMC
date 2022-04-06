namespace SharpMC.Util.Noise
{
    public class Simplex : NoiseGen
    {
       /* private readonly SimplexPerlin _simplexPerlin;
        public int Octaves { get; set; }
        public double Amplitude { get; set; }
        public double Persistance { get; set; }
        public double Frequency { get; set; }
        public double Lacunarity { get; set; }
        public Simplex(int seed)
        {
            Octaves = 2;
            Amplitude = 2;
            Persistance = 1;
            Frequency = 1;
            Lacunarity = 2;
            _simplexPerlin = new SimplexPerlin(seed, NoiseQuality.Best);
        }

        public override double Value2D(double x, double y)
        {
            double total = 0.0;
            double frequency = Frequency;
            double amplitude = Amplitude;

            for (int I = 0; I < Octaves; I++)
            {
                total += _simplexPerlin.GetValue((float) (x * frequency), (float) (y * frequency)) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Persistance;
            }
            return total;
        }

        public override double Value3D(double x, double y, double z)
        {
            double total = 0.0;
            double frequency = Frequency;
            double amplitude = Amplitude;

            for (int I = 0; I < Octaves; I++)
            {
                total += _simplexPerlin.GetValue((float) (x * frequency), (float) (y * frequency), (float) (z * frequency)) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Persistance;
            }
            return total;
        }*/
	    public override double Value2D(double x, double y)
	    {
		    throw new System.NotImplementedException();
	    }

	    public override double Value3D(double x, double y, double z)
	    {
		    throw new System.NotImplementedException();
	    }
    }
}
