namespace Haakjell.DecimalTime;

/// <summary>
/// Represents a time of day in decimal (metric) time format.
/// In decimal time: 1 day = 10 hours = 1,000 minutes = 100,000 seconds.
/// </summary>
public readonly struct DecTimeOnly : IComparable<DecTimeOnly>, IEquatable<DecTimeOnly>, IComparable
{
    private const double SecondsPerDay = 86400.0;
    private const double DecimalSecondsPerDay = 100000.0;
    private const double ConversionFactor = DecimalSecondsPerDay / SecondsPerDay;
    private const double InverseConversionFactor = SecondsPerDay / DecimalSecondsPerDay;

    private readonly TimeOnly _value;

    #region Constructors

    /// <summary>
    /// Initializes a new instance from a TimeOnly.
    /// </summary>
    public DecTimeOnly(TimeOnly timeOnly)
    {
        _value = timeOnly;
    }

    /// <summary>
    /// Initializes a new instance from standard time components.
    /// </summary>
    public DecTimeOnly(int hour, int minute, int second = 0, int millisecond = 0)
    {
        _value = new TimeOnly(hour, minute, second, millisecond);
    }

    #endregion

    #region Static Properties

    /// <summary>
    /// Gets the earliest possible time (midnight, 0:00:00 decimal).
    /// </summary>
    public static DecTimeOnly MinValue => new(TimeOnly.MinValue);

    /// <summary>
    /// Gets the latest possible time (just before midnight, 9:99:99 decimal).
    /// </summary>
    public static DecTimeOnly MaxValue => new(TimeOnly.MaxValue);

    /// <summary>
    /// Gets midnight (0:00:00 decimal).
    /// </summary>
    public static DecTimeOnly Midnight => new(new TimeOnly(0, 0, 0));

    /// <summary>
    /// Gets noon (5:00:00 decimal).
    /// </summary>
    public static DecTimeOnly Noon => new(new TimeOnly(12, 0, 0));

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a DecTimeOnly from a standard TimeOnly.
    /// </summary>
    public static DecTimeOnly FromTimeOnly(TimeOnly timeOnly) => new(timeOnly);

    /// <summary>
    /// Creates a DecTimeOnly from decimal time components.
    /// </summary>
    /// <param name="decHour">The decimal hour (0-9).</param>
    /// <param name="decMin">The decimal minute (0-99).</param>
    /// <param name="decSec">The decimal second (0-99).</param>
    public static DecTimeOnly FromDecimalTime(int decHour, int decMin, int decSec)
    {
        if (decHour < 0 || decHour > 9)
            throw new ArgumentOutOfRangeException(nameof(decHour), "Decimal hour must be between 0 and 9.");
        if (decMin < 0 || decMin > 99)
            throw new ArgumentOutOfRangeException(nameof(decMin), "Decimal minute must be between 0 and 99.");
        if (decSec < 0 || decSec > 99)
            throw new ArgumentOutOfRangeException(nameof(decSec), "Decimal second must be between 0 and 99.");

        var totalDecimalSeconds = decHour * 10000.0 + decMin * 100.0 + decSec;
        var totalStandardSeconds = totalDecimalSeconds * InverseConversionFactor;

        return new DecTimeOnly(TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(totalStandardSeconds)));
    }

    #endregion

    #region Standard Time Properties (pass-through)

    /// <summary>Gets the standard hour component (0-23).</summary>
    public int Hour => _value.Hour;

    /// <summary>Gets the standard minute component (0-59).</summary>
    public int Minute => _value.Minute;

    /// <summary>Gets the standard second component (0-59).</summary>
    public int Second => _value.Second;

    /// <summary>Gets the standard millisecond component (0-999).</summary>
    public int Millisecond => _value.Millisecond;

    /// <summary>Gets the ticks representing this time.</summary>
    public long Ticks => _value.Ticks;

    #endregion

    #region Decimal Time Properties

    /// <summary>
    /// Gets the total decimal seconds since midnight (0 to 99999.999...).
    /// </summary>
    public double TotalDecimalSeconds => _value.ToTimeSpan().TotalSeconds * ConversionFactor;

    /// <summary>
    /// Gets the decimal hour component (0-9).
    /// </summary>
    public int DecimalHour => (int)(TotalDecimalSeconds / 10000);

    /// <summary>
    /// Gets the decimal minute component (0-99).
    /// </summary>
    public int DecimalMinute => (int)(TotalDecimalSeconds / 100) % 100;

    /// <summary>
    /// Gets the decimal second component (0-99).
    /// </summary>
    public int DecimalSecond => (int)TotalDecimalSeconds % 100;

    #endregion

    #region Core Methods

    /// <summary>
    /// Converts this DecTimeOnly to a standard TimeOnly.
    /// </summary>
    public TimeOnly ToTimeOnly() => _value;

    /// <summary>
    /// Returns a string representation of the decimal time in "h:mm:ss" format.
    /// </summary>
    public string ToDecimalTimeString()
    {
        return $"{DecimalHour}:{DecimalMinute:D2}:{DecimalSecond:D2}";
    }

    /// <summary>
    /// Returns a string representation of this DecTimeOnly.
    /// </summary>
    public override string ToString() => ToDecimalTimeString();

    #endregion

    #region Comparison and Equality

    /// <inheritdoc />
    public bool Equals(DecTimeOnly other) => _value.Equals(other._value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is DecTimeOnly other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <inheritdoc />
    public int CompareTo(DecTimeOnly other) => _value.CompareTo(other._value);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is DecTimeOnly other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(DecTimeOnly)}");
    }

    public static bool operator ==(DecTimeOnly left, DecTimeOnly right) => left.Equals(right);
    public static bool operator !=(DecTimeOnly left, DecTimeOnly right) => !left.Equals(right);
    public static bool operator <(DecTimeOnly left, DecTimeOnly right) => left.CompareTo(right) < 0;
    public static bool operator >(DecTimeOnly left, DecTimeOnly right) => left.CompareTo(right) > 0;
    public static bool operator <=(DecTimeOnly left, DecTimeOnly right) => left.CompareTo(right) <= 0;
    public static bool operator >=(DecTimeOnly left, DecTimeOnly right) => left.CompareTo(right) >= 0;

    #endregion

    #region Conversion Operators

    /// <summary>
    /// Implicitly converts a DecTimeOnly to a TimeOnly.
    /// </summary>
    public static implicit operator TimeOnly(DecTimeOnly decTimeOnly) => decTimeOnly._value;

    /// <summary>
    /// Explicitly converts a TimeOnly to a DecTimeOnly.
    /// </summary>
    public static explicit operator DecTimeOnly(TimeOnly timeOnly) => new(timeOnly);

    #endregion

    #region Arithmetic Operators

    /// <summary>
    /// Adds a DecTimeSpan to a DecTimeOnly.
    /// </summary>
    public static DecTimeOnly operator +(DecTimeOnly timeOnly, DecTimeSpan timeSpan)
    {
        return new DecTimeOnly(timeOnly._value.Add(timeSpan.ToTimeSpan()));
    }

    /// <summary>
    /// Subtracts a DecTimeSpan from a DecTimeOnly.
    /// </summary>
    public static DecTimeOnly operator -(DecTimeOnly timeOnly, DecTimeSpan timeSpan)
    {
        return new DecTimeOnly(timeOnly._value.Add(-timeSpan.ToTimeSpan()));
    }

    /// <summary>
    /// Subtracts two DecTimeOnly values and returns a DecTimeSpan.
    /// </summary>
    public static DecTimeSpan operator -(DecTimeOnly left, DecTimeOnly right)
    {
        return DecTimeSpan.FromTimeSpan(left._value - right._value);
    }

    #endregion
}
