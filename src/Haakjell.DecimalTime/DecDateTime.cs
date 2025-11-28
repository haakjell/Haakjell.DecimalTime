namespace Haakjell.DecimalTime;

public sealed class DecDateTime
{
    public DateTime Value { get; private init; }

    public static DecDateTime FromDateTime(DateTime dateTime) => new() { Value = dateTime };

    private static bool EqualOperator(DecDateTime? left, DecDateTime? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;
        var other = (DecDateTime)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public static bool operator ==(DecDateTime left, DecDateTime right) => EqualOperator(left, right);

    public static bool operator !=(DecDateTime left, DecDateTime right) => !EqualOperator(left, right);

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }

    private IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}