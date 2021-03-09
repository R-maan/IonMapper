using System;
using Amazon.IonDotnet.Tree;
using Amazon.IonDotnet.Tree.Impl;

namespace IonMapper
{
    public class Lossy : IIonMapper
    {
        private static IValueFactory factory = new ValueFactory();
        internal IMapper mapper;

        internal Lossy()
        {
            this.mapper = new LossyMapper();
        }

        internal Lossy(IMapper mapp) {
            this.mapper = mapp;
        }

        public IMapper GetMapper()
        {
            return mapper;
        }

        public IIonValue ToIon<T>(T obj)
        {
            var ser = new Serializer(o =>
            {
                switch (Type.GetTypeCode(o.GetType()))
                {
                    case TypeCode.String:
                        return factory.NewString((string)o);
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.Int64:
                        return factory.NewInt((int)o);
                    case TypeCode.Boolean:
                        return factory.NewBool((bool)o);
                    // TODO: define more types
                    default:
                        throw new Exception("Object type is not supported");
                }
            });

            return ser.FromObjectToIon<T>(obj);
        }

        public T FromIon<T>(IIonValue ionValue)
        {
            var des = new Deserializer((t, i) => {
                switch (Type.GetTypeCode(t))
                {
                    case TypeCode.String:
                        return mapper.MapFromIonStringTo<string>(i);
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.Int64:
                        return mapper.MapFromIonIntTo<int>(i);
                    case TypeCode.Boolean:
                        return mapper.MapFromIonBoolTo<bool>(i);
                    // TODO: define more types
                    default:
                        throw new Exception("Object type is not supported");
                }
            });

            return des.FromIonToType<T>(ionValue);
        }
    }
}
