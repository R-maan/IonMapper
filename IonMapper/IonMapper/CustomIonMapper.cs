using Amazon.IonDotnet.Tree;
using System;
using System.Reflection;
using System.Collections.Generic;
using Amazon.IonDotnet;
using static IonMapper.IonMapperFactory;

namespace IonMapper
{
    public class CustomIonMapper : IMapper, IIonMapper
    {
        private IIonMapper defaultMapper;
        private IMapper mapper;
        private Func<IIonValue, object> IIonStringTo;
        private Func<IIonValue, object> IIonIntTo;
        private Func<IIonValue, object> IIonBoolTo;

        public CustomIonMapper(MapperTypes defaultMapperType)
        {
            this.defaultMapper = IonMapperFactory.GetMapper(defaultMapperType);
            this.mapper = defaultMapper.GetMapper();
        }


        public CustomIonMapper(MapperTypes defaultMapperType, IMapper customizedMapper)
        {
            this.defaultMapper = IonMapperFactory.GetMapper(defaultMapperType);
            this.mapper = customizedMapper;
        }

        public CustomIonMapper WithMapperFromIonTo<T>(Func<IIonInt, object> func)
        {
            IIonIntTo = func;
            return this;
        }

        public CustomIonMapper WithMapperFromIonTo<T>(Func<IIonString, object> func)
        {
            IIonStringTo = func;
            return this;
        }

        public CustomIonMapper WithMapperFromIonTo<T>(Func<IIonBool, object> func)
        {
            IIonBoolTo= func;
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

            return mapper.MapFromIonIntTo<T>(ionValue);
        }

        public T MapFromIonStringTo<T>(IIonValue ionValue)
        {
            if (IIonStringTo != null)
            {
                return (T)IIonStringTo(ionValue);
            }

            return mapper.MapFromIonStringTo<T>(ionValue);
        }

        public IIonValue ToIon<T>(T value)
        {
            return defaultMapper.ToIon(value);
        }

        public T FromIon<T>(IIonValue ionValue)
        {
            Object obj;

            if (ionValue.Type() == IonType.Datagram)
            {
                ionValue = ionValue.GetElementAt(0);
            }

            if (Type.GetTypeCode(typeof(T)) == TypeCode.Object)
            {
                obj = DeserializeObject(typeof(T), ionValue);
                return (T)obj;
            }
            else
            {
                obj = DeserializeNonObject(typeof(T), ionValue);
                return (T)obj;
            }
        }

        internal object DeserializeObject(Type type, IIonValue ionValue)
        {
            Object obj;
            // Get type's attributes
            IList<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());

            // Array to hold parameters to create object
            object[] paramArray = new object[properties.Count];

            int count = 0;
            foreach (PropertyInfo property in properties)
            {
                Type propertyType = property.PropertyType;
                if (Type.GetTypeCode(propertyType) == TypeCode.Object)
                {
                    obj = DeserializeObject(propertyType, ionValue.GetField(property.Name));
                }
                else
                {
                    obj = DeserializeNonObject(propertyType, ionValue.GetField(property.Name));
                }
                paramArray[count] = obj;
                count++;
            }

            // Create object by passing in array of parameters into constructor
            obj = Activator.CreateInstance(type, args: paramArray);
            return obj;
        }

        internal object DeserializeNonObject(Type type, IIonValue ionValue)
        {
            // Object is a primitive type
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return MapFromIonStringTo<string>(ionValue);
                case TypeCode.Int32:
                    return MapFromIonIntTo<int>(ionValue);
                // TODO: define more types
                default:
                    throw new Exception("Object type is not supported");
            }
        }
    }
}
