namespace Paperial
{
    public static class GridUtils
    {
        public static int CoordinatesToIndex(int x, int y, int z, int size)
        {
            return x + (y * size) + (z * size * size);
        }

        public static int GetGridFlagCount(int sizeOfGridAxis)
        {
            return sizeOfGridAxis * sizeOfGridAxis * sizeOfGridAxis;
        }

        public static ulong GetHugeGridFlagCount(int sizeOfGridAxis)
        {
            return (ulong)sizeOfGridAxis * (ulong)sizeOfGridAxis * (ulong)sizeOfGridAxis;
        }

        public static long[] GenerateFlagsArray(int numberNeeded)
        {
            return new long[numberNeeded / 64];
        }
    }
}