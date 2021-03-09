using System;

namespace IonMapper
{
    public static class IonMapper
    {
        public static IIonMapper LOSSY()
        {
            return IonMapperFactory.GetMapper(MapperTypes.LOSSY);
        }

        public static IIonMapper STRICT()
        {
            throw new NotImplementedException(); ;
        }

        public static IIonMapper ION()
        {
            throw new NotImplementedException(); ;
        }
    }
}
