namespace Haakjell.DecimalTime;

/// <summary>
/// Represents a time interval in decimal (metric) time.
/// In decimal time: 1 day = 10 hours = 1,000 minutes = 100,000 seconds.
/// </summary>
public readonly struct DecTimeSpan : IComparable<DecTimeSpan>, IEquatable<DecTimeSpan>, IComparable
{
    private const double SecondsPerDay = 86400.0;
    private const double DecimalSecondsPerDay = 100000.0;
    private const double ConversionFactor = DecimalSecondsPerDay / SecondsPerDay;
    private const double InverseConversionFactor = SecondsPerDay / DecimalSecondsPerDay;

    private readonly TimeSpan _value;

    #region Constructors

    /// <summary>
    /// Initializes a new instance from a TimeSpan.
    /// </summary>
    public DecTimeSpan(TimeSpan timeSpan)
    {
        _value = timeSpan;
    }

    /// <summary>
    /// Initializes a new instance from decimal time components.
    /// </summary>
    /// <param name="days">The number of days.</param>
    /// <param name="decHours">The decimal hours (0-9 per day).</param>
    /// <param name="decMinutes">The decimal minutes (0-99 per hour).</param>
    /// <param name="decSeconds">The decimal seconds (0-99 per minute).</param>
    public DecTimeSpan(int days, int decHours, int decMinutes, int decSeconds)
    {
        double totalDecimalSeconds = days * DecimalSecondsPerDay
            + decHours * 10000.0
            + decMinutes * 100.0
            + decSeconds;
        double totalStandardSeconds = totalDecimalSeconds * InverseConversionFactor;
        _value = TimeSpan.FromSeconds(totalStandardSeconds);
    }

    #endregion

    #region Static Properties

    /// <summary>
    /// Gets a DecTimeSpan representing zero duration.
    /// </summary>
    public static DecTimeSpan Zero => new(TimeSpan.Zero);

    /// <summary>
    /// Gets the minimum value of DecTimeSpan.
    /// </summary>
    public static DecTimeSpan MinValue => new(TimeSpan.MinValue);

    /// <summary>
    /// Gets the maximum value of DecTimeSpan.
    /// </summary>
    public static DecTimeSpan MaxValue => new(TimeSpan.MaxValue);

    #endregion

    #region Factory Methods

    /// <summary>
    /// Creates a DecTimeSpan from a standard TimeSpan.
    /// </summary>
    public static DecTimeSpan FromTimeSpan(TimeSpan timeSpan) => new(timeSpan);

    /// <summary>
    /// Creates a DecTimeSpan from decimal hours.
    /// </summary>
    public static DecTimeSpan FromDecimalHours(double decimalHours)
    {
        double totalDecimalSeconds = decimalHours * 10000.0;
        double totalStandardSeconds = totalDecimalSeconds * InverseConversionFactor;
        return new DecTimeSpan(TimeSpan.FromSeconds(totalStandardSeconds));
    }

    /// <summary>
    /// Creates a DecTimeSpan from decimal minutes.
    /// </summary>
    public static DecTimeSpan FromDecimalMinutes(double decimalMinutes)
    {
        double totalDecimalSeconds = decimalMinutes * 100.0;
        double totalStandardSeconds = totalDecimalSeconds * InverseConversionFactor;
        return new DecTimeSpan(TimeSpan.FromSeconds(totalStandardSeconds));
    }

    /// <summary>
    /// Creates a DecTimeSpan from decimal seconds.
    /// </summary>
    public static DecTimeSpan FromDecimalSeconds(double decimalSeconds)
    {
        double totalStandardSeconds = decimalSeconds * InverseConversionFactor;
        return new DecTimeSpan(TimeSpan.FromSeconds(totalStandardSeconds));
    }

    #endregion

    #region Component Properties

    /// <summary>
    /// Gets the days component of the time interval.
    /// </summary>
    public int Days => _value.Days;

    /// <summary>
    /// Gets the decimal hours component (0-9 per day).
    /// </summary>
    public int DecimalHours
    {
        get
        {
            double totalDecSecs = Math.Abs(_value.TotalSeconds * ConversionFactor);
            double dayFraction = totalDecSecs % DecimalSecondsPerDay;
            return (int)(dayFraction / 10000);
        }
    }

    /// <summary>
    /// Gets the decimal minutes component (0-99 per hour).
    /// </summary>
    public int DecimalMinutes
    {
        get
        {
            double totalDecSecs = Math.Abs(_value.TotalSeconds * ConversionFactor);
            double dayFraction = totalDecSecs % DecimalSecondsPerDay;
            return (int)(dayFraction / 100) % 100;
        }
    }

    /// <summary>
    /// Gets the decimal seconds component (0-99 per minute).
    /// </summary>
    public int DecimalSeconds
    {
        get
        {
            double totalDecSecs = Math.Abs(_value.TotalSeconds * ConversionFactor);
            return (int)totalDecSecs % 100;
        }
    }

    #endregion

    #region Total Properties

    /// <summary>
    /// Gets the total number of days represented by this interval.
    /// </summary>
    public double TotalDays => _value.TotalDays;

    /// <summary>
    /// Gets the total number of decimal hours represented by this interval.
    /// </summary>
    public double TotalDecimalHours => _value.TotalSeconds * ConversionFactor / 10000.0;

    /// <summary>
    /// Gets the total number of decimal minutes represented by this interval.
    /// </summary>
    public double TotalDecimalMinutes => _value.TotalSeconds * ConversionFactor / 100.0;

    /// <summary>
    /// Gets the total number of decimal seconds represented by this interval.
    /// </summary>
    public double TotalDecimalSeconds => _value.TotalSeconds * ConversionFactor;

    #endregion

    #region Core Methods

    /// <summary>
    /// Converts this DecTimeSpan to a standard TimeSpan.
    /// </summary>
    public TimeSpan ToTimeSpan() => _value;

    /// <summary>
    /// Returns a string representation of this DecTimeSpan.
    /// </summary>
    public override string ToString()
    {
        var sign = _value < TimeSpan.Zero ? "-" : "";
        return $"{sign}{Math.Abs(Days)}.{DecimalHours}:{DecimalMinutes:D2}:{DecimalSeconds:D2}";
    }

    #endregion

    #region Comparison and Equality

    /// <inheritdoc />
    public bool Equals(DecTimeSpan other) => _value.Equals(other._value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is DecTimeSpan other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <inheritdoc />
    public int CompareTo(DecTimeSpan other) => _value.CompareTo(other._value);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is DecTimeSpan other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(DecTimeSpan)}");
    }

    public static bool operator ==(DecTimeSpan left, DecTimeSpan right) => left.Equals(right);
    public static bool operator !=(DecTimeSpan left, DecTimeSpan right) => !left.Equals(right);
    public static bool operator <(DecTimeSpan left, DecTimeSpan right) => left.CompareTo(right) < 0;
    public static bool operator >(DecTimeSpan left, DecTimeSpan right) => left.CompareTo(right) > 0;
    public static bool operator <=(DecTimeSpan left, DecTimeSpan right) => left.CompareTo(right) <= 0;
    public static bool operator >=(DecTimeSpan left, DecTimeSpan right) => left.CompareTo(right) >= 0;

    #endregion

    #region Arithmetic Operators

    /// <summary>
    /// Adds two DecTimeSpan values.
    /// </summary>
    public static DecTimeSpan operator +(DecTimeSpan left, DecTimeSpan right)
    {
        return new DecTimeSpan(left._value + right._value);
    }

    /// <summary>
    /// Subtracts one DecTimeSpan from another.
    /// </summary>
    public static DecTimeSpan operator -(DecTimeSpan left, DecTimeSpan right)
    {
        return new DecTimeSpan(left._value - right._value);
    }

    /// <summary>
    /// Negates a DecTimeSpan.
    /// </summary>
    public static DecTimeSpan operator -(DecTimeSpan value)
    {
        return new DecTimeSpan(-value._value);
    }

    /// <summary>
    /// Returns the positive value (unary plus).
    /// </summary>
    public static DecTimeSpan operator +(DecTimeSpan value)
    {
        return value;
    }

    #endregion

    #region Conversion Operators

    /// <summary>
    /// Implicitly converts a DecTimeSpan to a TimeSpan.
    /// </summary>
    public static implicit operator TimeSpan(DecTimeSpan decTimeSpan) => decTimeSpan._value;

    /// <summary>
    /// Explicitly converts a TimeSpan to a DecTimeSpan.
    /// </summary>
    public static explicit operator DecTimeSpan(TimeSpan timeSpan) => new(timeSpan);

    #endregion
}
