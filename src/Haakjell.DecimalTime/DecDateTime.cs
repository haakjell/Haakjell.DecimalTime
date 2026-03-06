namespace Haakjell.DecimalTime;

/// <summary>
/// Represents a date and time with decimal (metric) time-of-day representation.
/// In decimal time: 1 day = 10 hours = 1,000 minutes = 100,000 seconds.
/// </summary>
public readonly struct DecDateTime : IComparable<DecDateTime>, IEquatable<DecDateTime>, IComparable
{
    private const double SecondsPerDay = 86400.0;
    private const double DecimalSecondsPerDay = 100000.0;
    private const double ConversionFactor = DecimalSecondsPerDay / SecondsPerDay;
    private const double InverseConversionFactor = SecondsPerDay / DecimalSecondsPerDay;

    private readonly DateTime _value;

    #region Constructors

    /// <summary>
    /// Initializes a new instance from a DateTime.
    /// </summary>
    public DecDateTime(DateTime dateTime)
    {
        _value = dateTime;
    }

    /// <summary>
    /// Initializes a new instance from standard time components.
    /// </summary>
    public DecDateTime(int year, int month, int day, int hour, int minute, int second)
    {
        _value = new DateTime(year, month, day, hour, minute, second);
    }

    /// <summary>
    /// Initializes a new instance from ticks.
    /// </summary>
    public DecDateTime(long ticks)
    {
        _value = new DateTime(ticks);
    }

    #endregion

    #region Static Properties

    /// <summary>
    /// Gets a DecDateTime representing the current local date and time.
    /// </summary>
    public static DecDateTime Now => new(DateTime.Now);

    /// <summary>
    /// Gets a DecDateTime representing the current UTC date and time.
    /// </summary>
    public static DecDateTime UtcNow => new(DateTime.UtcNow);

    /// <summary>
    /// Gets a DecDateTime representing today's date with time set to midnight.
    /// </summary>
    public static DecDateTime Today => new(DateTime.Today);

    /// <summary>
    /// Gets the minimum value of DecDateTime.
    /// </summary>
    public static DecDateTime MinValue => new(DateTime.MinValue);

    /// <summary>
    /// Gets the maximum value of DecDateTime.
    /// </summary>
    public static DecDateTime MaxValue => new(DateTime.MaxValue);

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a DecDateTime from a standard DateTime.
    /// </summary>
    public static DecDateTime FromDateTime(DateTime dateTime) => new(dateTime);

    /// <summary>
    /// Creates a DecDateTime from decimal time components.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month (1-12).</param>
    /// <param name="day">The day of month.</param>
    /// <param name="decHour">The decimal hour (0-9).</param>
    /// <param name="decMin">The decimal minute (0-99).</param>
    /// <param name="decSec">The decimal second (0-99).</param>
    public static DecDateTime FromDecimalTime(int year, int month, int day, int decHour, int decMin, int decSec)
    {
        if (decHour < 0 || decHour > 9)
            throw new ArgumentOutOfRangeException(nameof(decHour), "Decimal hour must be between 0 and 9.");
        if (decMin < 0 || decMin > 99)
            throw new ArgumentOutOfRangeException(nameof(decMin), "Decimal minute must be between 0 and 99.");
        if (decSec < 0 || decSec > 99)
            throw new ArgumentOutOfRangeException(nameof(decSec), "Decimal second must be between 0 and 99.");

        var totalDecimalSeconds = decHour * 10000.0 + decMin * 100.0 + decSec;
        var totalStandardSeconds = totalDecimalSeconds * InverseConversionFactor;

        var date = new DateTime(year, month, day);
        var timeSpan = TimeSpan.FromSeconds(totalStandardSeconds);

        return new DecDateTime(date.Add(timeSpan));
    }

    #endregion

    #region Date Properties (pass-through)

    /// <summary>Gets the year component.</summary>
    public int Year => _value.Year;

    /// <summary>Gets the month component (1-12).</summary>
    public int Month => _value.Month;

    /// <summary>Gets the day of month component.</summary>
    public int Day => _value.Day;

    /// <summary>Gets the day of week.</summary>
    public DayOfWeek DayOfWeek => _value.DayOfWeek;

    /// <summary>Gets the day of year (1-366).</summary>
    public int DayOfYear => _value.DayOfYear;

    /// <summary>Gets the date component with time set to midnight.</summary>
    public DecDateTime Date => new(_value.Date);

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

    /// <summary>Gets the ticks representing this date and time.</summary>
    public long Ticks => _value.Ticks;

    /// <summary>Gets the DateTimeKind (Local, Utc, or Unspecified).</summary>
    public DateTimeKind Kind => _value.Kind;

    /// <summary>Gets the time of day as a TimeSpan.</summary>
    public TimeSpan TimeOfDay => _value.TimeOfDay;

    #endregion

    #region Decimal Time Properties

    /// <summary>
    /// Gets the total decimal seconds since midnight (0 to 99999.999...).
    /// </summary>
    public double TotalDecimalSeconds => _value.TimeOfDay.TotalSeconds * ConversionFactor;

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
    /// Converts this DecDateTime to a standard DateTime.
    /// </summary>
    public DateTime ToDateTime() => _value;

    /// <summary>
    /// Returns a string representation of the decimal time in "h:mm:ss" format.
    /// </summary>
    public string ToDecimalTimeString()
    {
        return $"{DecimalHour}:{DecimalMinute:D2}:{DecimalSecond:D2}";
    }

    /// <summary>
    /// Returns a string representation of this DecDateTime.
    /// </summary>
    public override string ToString()
    {
        return $"{_value:yyyy-MM-dd} {ToDecimalTimeString()}";
    }

    #endregion

    #region Comparison and Equality

    /// <inheritdoc />
    public bool Equals(DecDateTime other) => _value.Equals(other._value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is DecDateTime other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <inheritdoc />
    public int CompareTo(DecDateTime other) => _value.CompareTo(other._value);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is DecDateTime other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(DecDateTime)}");
    }

    public static bool operator ==(DecDateTime left, DecDateTime right) => left.Equals(right);
    public static bool operator !=(DecDateTime left, DecDateTime right) => !left.Equals(right);
    public static bool operator <(DecDateTime left, DecDateTime right) => left.CompareTo(right) < 0;
    public static bool operator >(DecDateTime left, DecDateTime right) => left.CompareTo(right) > 0;
    public static bool operator <=(DecDateTime left, DecDateTime right) => left.CompareTo(right) <= 0;
    public static bool operator >=(DecDateTime left, DecDateTime right) => left.CompareTo(right) >= 0;

    #endregion

    #region Conversion Operators

    /// <summary>
    /// Implicitly converts a DecDateTime to a DateTime.
    /// </summary>
    public static implicit operator DateTime(DecDateTime decDateTime) => decDateTime._value;

    /// <summary>
    /// Explicitly converts a DateTime to a DecDateTime.
    /// </summary>
    public static explicit operator DecDateTime(DateTime dateTime) => new(dateTime);

    #endregion

    #region Arithmetic Operators

    /// <summary>
    /// Adds a DecTimeSpan to a DecDateTime.
    /// </summary>
    public static DecDateTime operator +(DecDateTime dateTime, DecTimeSpan timeSpan)
    {
        return new DecDateTime(dateTime._value.Add(timeSpan.ToTimeSpan()));
    }

    /// <summary>
    /// Subtracts a DecTimeSpan from a DecDateTime.
    /// </summary>
    public static DecDateTime operator -(DecDateTime dateTime, DecTimeSpan timeSpan)
    {
        return new DecDateTime(dateTime._value.Subtract(timeSpan.ToTimeSpan()));
    }

    /// <summary>
    /// Subtracts two DecDateTime values and returns a DecTimeSpan.
    /// </summary>
    public static DecTimeSpan operator -(DecDateTime left, DecDateTime right)
    {
        return DecTimeSpan.FromTimeSpan(left._value - right._value);
    }

    #endregion
}
