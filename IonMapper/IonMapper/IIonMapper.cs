using Amazon.IonDotnet.Tree;

namespace IonMapper
{
    public interface IIonMapper
    {
        IIonValue ToIon<T>(T value);

        T FromIon<T>(IIonValue input);

        IMapper GetMapper();
    }
}
