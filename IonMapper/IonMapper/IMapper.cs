using Amazon.IonDotnet.Tree;

namespace IonMapper
{
    public interface IMapper
    {
        T MapFromIonStringTo<T>(IIonValue ionValue);

        T MapFromIonIntTo<T>(IIonValue ionValue);

        T MapFromIonBoolTo<T>(IIonValue ionValue);
    }
}
