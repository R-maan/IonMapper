using Amazon.IonDotnet.Builders;
using Amazon.IonDotnet.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IonMapper.Test
{
    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public void TestMapper()
        {
            IIonValue ionStruct = IonLoader.Default
                .Load("{name: \"John\", age: 22, address: {city: \"cityName\", street: \"streetName\", isApartment: false}}");

            var lossyIonMapper = IonMapper.LOSSY();
            Person person = lossyIonMapper.FromIon<Person>(ionStruct);
            Console.WriteLine(person.ToString());

            IIonValue ionValue = lossyIonMapper.ToIon<Person>(person);
            Console.WriteLine(ionValue.ToPrettyString());

            Console.WriteLine("\n \n ==== Now with a customized mapper === \n");

            Func<IIonString, string> specialIonStringMapper = ionValue => (ionValue.StringValue).ToUpper();
            Func<IIonInt, int> specialIonIntMapper = ionValue => (ionValue.IntValue) * 3;
            Func<IIonBool, bool> specialIonBoolMapper = ionValue => !(ionValue.BoolValue);

            var customizedMapper = new CustomIonMapper(MapperTypes.LOSSY)
                .WithMapperFromIonTo<string>(specialIonStringMapper)
                .WithMapperFromIonTo<int>(specialIonIntMapper)
                .WithMapperFromIonTo<bool>(specialIonBoolMapper)
                .Build();

            person = customizedMapper.FromIon<Person>(ionStruct);
            Console.WriteLine(person.ToString());

            ionValue = customizedMapper.ToIon<Person>(person);
            Console.WriteLine(ionValue.ToPrettyString());
        }
    }

    internal class Person
    {
        public Person(string name, int age, Address address)
        {
            this.name = name;
            this.age = age;
            this.address = address;
        }

        public string name { get; }

        public int age { get; }

        public Address address { get; }

        public override string ToString()
        {
            return "Person\t " + 
                "FirstName: " + this.name + "   " + 
                "Age: " + this.age + "  " + 
                "\n" + this.address.ToString();
        }
    }

    class Address
    {
        public Address(string city, string street, bool isApartment)
        {
            this.city = city;
            this.street = street;
            this.isApartment = isApartment;
        }

        public string city { get; }
        public string street { get; }
        public bool isApartment { get; }

        public override string ToString()
        {
            return "Address\t " +
                "city: " + this.city + "   " +
                "street: " + this.street + "   " +
                "isApartment: " + this.isApartment;
        }
    }
}
