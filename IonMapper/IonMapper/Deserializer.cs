using Amazon.IonDotnet;
using Amazon.IonDotnet.Tree;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IonMapper
{
    internal class Deserializer
    {
        private readonly Func<Type, IIonValue, object> DeserializingMappers;

        internal Deserializer(Func<Type, IIonValue, object> deserializingMappers)
        {
            this.DeserializingMappers = deserializingMappers;
        }

        internal T FromIonToType<T>(IIonValue ionValue)
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
                obj = DeserializingMappers(typeof(T), ionValue);
                return (T)obj;
            }
        }

        private object DeserializeObject(Type type, IIonValue ionValue)
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
                    obj = DeserializingMappers(propertyType, ionValue.GetField(property.Name));
                }
                paramArray[count] = obj;
                count++;
            }

            // Create object by passing in array of parameters into constructor
            obj = Activator.CreateInstance(type, args: paramArray);
            return obj;
        }
    }
}
