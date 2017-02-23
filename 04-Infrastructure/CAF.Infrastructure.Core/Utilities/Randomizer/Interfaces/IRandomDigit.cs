namespace CAF.Infrastructure.Core.Utilities.Randomizer.Interfaces
{
    public interface IRandomDigit<in TSeed, TType>
        : IRandomValueType<TSeed, TType>
        where TType : struct
        where TSeed : new()
    {
        TType GeneratePositiveValue();

        TType GenerateNegativeValue();
    }
}
