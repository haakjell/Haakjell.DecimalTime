namespace Haakjell.DecimalTime;

/// <summary>
/// Extension methods for converting standard .NET time types to decimal time.
/// </summary>
public static class DecDateTimeExtensions
{
    /// <summary>
    /// Converts a DateTime to a DecDateTime.
    /// </summary>
    public static DecDateTime ToDecDateTime(this DateTime dateTime) => new(dateTime);

    /// <summary>
    /// Converts a TimeSpan to a DecTimeSpan.
    /// </summary>
    public static DecTimeSpan ToDecTimeSpan(this TimeSpan timeSpan) => new(timeSpan);

    /// <summary>
    /// Converts a TimeOnly to a DecTimeOnly.
    /// </summary>
    public static DecTimeOnly ToDecTimeOnly(this TimeOnly timeOnly) => new(timeOnly);
}
