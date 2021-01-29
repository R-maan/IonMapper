
namespace IonMapper
{
    public enum MapperTypes : short
    {
        LOSSY = 0,
        DEFAULT = 1,
        JSON = 2,
        STRICT = 3,
        ION = 4,
    }

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
