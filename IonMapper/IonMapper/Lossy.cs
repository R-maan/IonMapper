using System;
using Amazon.IonDotnet.Tree;
using Amazon.IonDotnet.Tree.Impl;
using Amazon.IonDotnet;
using System.Reflection;
using System.Collections.Generic;

namespace IonMapper
{
    public class Lossy : IIonMapper
    {
        private static IValueFactory factory = new ValueFactory();
        internal IMapper mapp;

        internal Lossy()
        {
            this.mapp = new LossyMapper();
        }

        internal Lossy(IMapper mapp) {
            this.mapp = mapp;
        }

        public IMapper GetMapper()
        {
            return mapp;
        }

        public IIonValue ToIon<T>(T obj)
        {
            if (Type.GetTypeCode(obj.GetType()) == TypeCode.Object)
            {
                IIonValue emptyIonStruct = factory.NewEmptyStruct();
                return SerializeObject(obj, emptyIonStruct);
            }
            else
            {
                return SerializeNonObject(obj);
            }
        }

        public T FromIon<T>(IIonValue ionValue)
        {
            var l = new LossyMapper();
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
                    IIonValue propertyValue = SerializeNonObject(property.GetValue(obj, null));
                    ionValue.SetField(property.Name, propertyValue);
                }
            }
            return ionValue;
        }

        internal IIonValue SerializeNonObject(object obj)
        {
            // Object is a primitive type
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.String:
                    return factory.NewString((string)obj);
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.Int64:
                    return factory.NewInt((int)obj);
                // TODO: define more types
                default:
                    throw new Exception("Object type is not supported");
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
                    return mapp.MapFromIonStringTo<string>(ionValue);
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.Int64:
                    return mapp.MapFromIonIntTo<int>(ionValue);
                // TODO: define more types
                default:
                    throw new Exception("Object type is not supported");
            }
        }

    }
}
