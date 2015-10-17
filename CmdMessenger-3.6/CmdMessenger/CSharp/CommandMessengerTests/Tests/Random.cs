using System;

namespace CommandMessengerTests
{
    public class Random
    {
        private static readonly System.Random RandomNumber = new System.Random();

        public static float RandomizeFloat(float min, float max)
        {
            return (float)(min + ((max - min) *RandomNumber.NextDouble()));            
        }

        public static double RandomizeDouble(double min, double max)
        {
            var random = RandomNumber.NextDouble();
            return (double)(min + max * random - min * random);
        }


        public static Int16 RandomizeInt16(Int16 min, Int16 max)
        {
            return (Int16)(min + (((Double)max - (Double)min) *RandomNumber.NextDouble()));
        }

        public static Int32 RandomizeInt32(Int32 min, Int32 max)
        {
            return (Int32)(min + (((Double)max - (Double)min) *RandomNumber.NextDouble()));
        }

        public static bool RandomizeBool()
        {
            return (RandomNumber.NextDouble() > 0.5);
        }
    }
}