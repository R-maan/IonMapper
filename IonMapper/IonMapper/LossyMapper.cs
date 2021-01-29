using System;
using Amazon.IonDotnet.Tree;

namespace IonMapper
{
    internal class LossyMapper : IMapper
    {
        public T MapFromIonStringTo<T>(IIonValue ionValue)
        {
            var val = ionValue.StringValue;
            return (T)(Object)val;
        }

        public T MapFromIonIntTo<T>(IIonValue ionValue)
        {
            var val = ionValue.IntValue;
            return (T)(Object)val;
        }
    }
}
