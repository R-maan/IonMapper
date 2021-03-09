
namespace IonMapper
{
    public enum MapperTypes : short
    {
        LOSSY = 0,
        DEFAULT = 1,
        STRICT = 2,
        ION = 3,
    }

    /// Is it still needed? We can use <see cref="IonMapper"/> static methods where they return "new Lossy()" etc.
    public static class IonMapperFactory
    {
        public static IIonMapper GetMapper(MapperTypes mapperType)
        {
            if (mapperType == MapperTypes.DEFAULT || mapperType == MapperTypes.LOSSY) {
                return new Lossy();
            }
            return new Lossy();
        }
    }
}
