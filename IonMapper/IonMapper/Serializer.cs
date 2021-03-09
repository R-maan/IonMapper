using Amazon.IonDotnet.Tree;
using Amazon.IonDotnet.Tree.Impl;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IonMapper
{
    internal class Serializer
    {
        private static IValueFactory factory = new ValueFactory();
        private readonly Func<object, IIonValue> SerializingMappers;

        internal Serializer(Func<object, IIonValue> serializingMappers)
        {
            this.SerializingMappers = serializingMappers;
        }

        internal IIonValue FromObjectToIon<T>(T obj)
        {
            if (Type.GetTypeCode(obj.GetType()) == TypeCode.Object)
            {
                IIonValue emptyIonStruct = factory.NewEmptyStruct();
                return SerializeObject(obj, emptyIonStruct);
            }
            else
            {
                return SerializingMappers(obj);
            }
        }

        internal IIonValue SerializeObject(object obj, IIonValue ionValue)
        {
            // Get object attributes
            IList<PropertyInfo> properties = new List<PropertyInfo>(obj.GetType().GetProperties());

            foreach (var property in properties)
            {
                // If object attribute is another object
                if (Type.GetTypeCode(property.GetValue(obj, null).GetType()) == TypeCode.Object)
                {
                    IIonValue emptyIonStruct = factory.NewEmptyStruct();
                    IIonValue filledIonStruct = SerializeObject(property.GetValue(obj, null), emptyIonStruct);
                    ionValue.SetField(property.Name, filledIonStruct);
                }
                else
                {
                    IIonValue propertyValue = SerializingMappers(property.GetValue(obj, null));
                    ionValue.SetField(property.Name, propertyValue);
                }
            }
            return ionValue;
        }
    }
}
