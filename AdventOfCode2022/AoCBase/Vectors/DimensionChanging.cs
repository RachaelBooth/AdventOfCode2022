namespace AoCBase.Vectors
{
    public static class DimensionChanging
    {
        public static (int x, int y, int z) ToThreeDimensions(this (int x, int y) location)
        {
            return (location.x, location.y, 0);
        }

        public static (int x, int y, int z, int w) ToFourDimensions(this (int x, int y) location)
        {
            return (location.x, location.y, 0, 0);
        }
    }
}
