using System;

namespace SharpMC.World.Noises
{
    public class RandomGenerator : IRandomGenerator
    {
        private readonly Random _random;
        private readonly object _syncLock;

        public RandomGenerator() : this(new Random(), new object())
        {
        }

        public RandomGenerator(Random random, object syncLock)
        {
            _random = random;
            _syncLock = syncLock;
        }

        public int GetRandomNumber(int min, int max)
        {
            lock (_syncLock)
            {
                return _random.Next(min, max);
            }
        }
    }
}