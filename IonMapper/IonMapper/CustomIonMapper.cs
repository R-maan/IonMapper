using Amazon.IonDotnet.Tree;
using System;

namespace IonMapper
{
    public class CustomIonMapper : IMapper, IIonMapper
    {
        private readonly IIonMapper DefaultMapper;
        private readonly IMapper Mapper;

        delegate T CustomeIIonStringConverter<T>(IIonString ionString);
        delegate T CustomeIIonIntConverter<T>(IIonInt ionInt);
        delegate T CustomeIIonBoolConverter<T>(IIonBool ionBool);

        private Func<IIonValue, object> IIonStringTo;
        private Func<IIonValue, object> IIonIntTo;
        private Func<IIonValue, object> IIonBoolTo;

        public CustomIonMapper(MapperTypes defaultMapperType)
        {
            this.DefaultMapper = IonMapperFactory.GetMapper(defaultMapperType);
            this.Mapper = DefaultMapper.GetMapper();
        }

        public CustomIonMapper WithMapperFromIonTo<T>(Func<IIonInt, T> func)
        {
            T f(IIonInt x) => func(x);
            IIonIntTo = x => f(x);
            return this;
        }

        public CustomIonMapper WithMapperFromIonTo<T>(Func<IIonString, T> func)
        {
            T f(IIonString x) => func(x);
            IIonStringTo = x => f(x);
            return this;
        }

        public CustomIonMapper WithMapperFromIonTo<T>(Func<IIonBool, T> func)
        {
            T f(IIonBool x) => func(x);
            IIonBoolTo = x => f(x);
            return this;
        }

        public CustomIonMapper Build()
        {
            return this;
        }

        public IMapper GetMapper()
        {
            return null;
        }

        public T MapFromIonIntTo<T>(IIonValue ionValue)
        {
            if (IIonIntTo != null)
            {
                return (T)IIonIntTo(ionValue);
            }

            return Mapper.MapFromIonIntTo<T>(ionValue);
        }

        public T MapFromIonStringTo<T>(IIonValue ionValue)
        {
            if (IIonStringTo != null)
            {
                return (T)IIonStringTo(ionValue);
            }

            return Mapper.MapFromIonStringTo<T>(ionValue);
        }

        public T MapFromIonBoolTo<T>(IIonValue ionValue)
        {
            if (IIonBoolTo != null)
            {
                return (T)IIonBoolTo(ionValue);
            }

            return Mapper.MapFromIonBoolTo<T>(ionValue);
        }

        public IIonValue ToIon<T>(T value)
        {
            return DefaultMapper.ToIon(value);
        }

        public T FromIon<T>(IIonValue ionValue)
        {
            var des = new Deserializer((t, i) => {
                switch (Type.GetTypeCode(t))
                {
                    case TypeCode.String:
                        return this.MapFromIonStringTo<string>(i);
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.Int64:
                        return this.MapFromIonIntTo<int>(i);
                    case TypeCode.Boolean:
                        return this.MapFromIonBoolTo<bool>(i);
                    // TODO: define more types
                    default:
                        throw new Exception("Object type is not supported");
                }
            });

            return des.FromIonToType<T>(ionValue);
        }
    }
}
