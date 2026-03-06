using Xunit;

namespace Haakjell.DecimalTime.Tests;

public class DecDateTimeTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_FromDateTime_PreservesValue()
    {
        var dateTime = new DateTime(2024, 6, 15, 12, 0, 0);
        var decDateTime = new DecDateTime(dateTime);

        Assert.Equal(dateTime, decDateTime.ToDateTime());
    }

    [Fact]
    public void Constructor_FromComponents_CreatesCorrectDateTime()
    {
        var decDateTime = new DecDateTime(2024, 6, 15, 12, 30, 45);

        Assert.Equal(2024, decDateTime.Year);
        Assert.Equal(6, decDateTime.Month);
        Assert.Equal(15, decDateTime.Day);
        Assert.Equal(12, decDateTime.Hour);
        Assert.Equal(30, decDateTime.Minute);
        Assert.Equal(45, decDateTime.Second);
    }

    [Fact]
    public void Constructor_FromTicks_CreatesCorrectDateTime()
    {
        var dateTime = new DateTime(2024, 6, 15, 12, 0, 0);
        var decDateTime = new DecDateTime(dateTime.Ticks);

        Assert.Equal(dateTime, decDateTime.ToDateTime());
    }

    #endregion

    #region Static Properties Tests

    [Fact]
    public void Now_ReturnsCurrentTime()
    {
        var before = DateTime.Now;
        var decNow = DecDateTime.Now;
        var after = DateTime.Now;

        Assert.True(decNow.ToDateTime() >= before);
        Assert.True(decNow.ToDateTime() <= after);
    }

    [Fact]
    public void UtcNow_ReturnsUtcTime()
    {
        var decUtcNow = DecDateTime.UtcNow;

        Assert.Equal(DateTimeKind.Utc, decUtcNow.Kind);
    }

    [Fact]
    public void Today_ReturnsDateWithMidnight()
    {
        var decToday = DecDateTime.Today;

        Assert.Equal(0, decToday.Hour);
        Assert.Equal(0, decToday.Minute);
        Assert.Equal(0, decToday.Second);
        Assert.Equal(0, decToday.DecimalHour);
    }

    [Fact]
    public void MinValue_MatchesDateTimeMinValue()
    {
        Assert.Equal(DateTime.MinValue, DecDateTime.MinValue.ToDateTime());
    }

    [Fact]
    public void MaxValue_MatchesDateTimeMaxValue()
    {
        Assert.Equal(DateTime.MaxValue, DecDateTime.MaxValue.ToDateTime());
    }

    #endregion

    #region Decimal Time Conversion Tests

    [Fact]
    public void Midnight_IsZeroDecimalTime()
    {
        var midnight = new DecDateTime(2024, 1, 1, 0, 0, 0);

        Assert.Equal(0, midnight.DecimalHour);
        Assert.Equal(0, midnight.DecimalMinute);
        Assert.Equal(0, midnight.DecimalSecond);
        Assert.Equal(0, midnight.TotalDecimalSeconds, 1);
    }

    [Fact]
    public void Noon_IsFiveDecimalHours()
    {
        var noon = new DecDateTime(2024, 1, 1, 12, 0, 0);

        Assert.Equal(5, noon.DecimalHour);
        Assert.Equal(0, noon.DecimalMinute);
        Assert.Equal(0, noon.DecimalSecond);
        Assert.Equal(50000, noon.TotalDecimalSeconds, 1);
    }

    [Fact]
    public void EndOfDay_IsNineDecimalHours()
    {
        // 23:59:59 should be close to 9:99:99 in decimal time
        var almostMidnight = new DecDateTime(2024, 1, 1, 23, 59, 59);

        Assert.Equal(9, almostMidnight.DecimalHour);
        Assert.Equal(99, almostMidnight.DecimalMinute);
        Assert.Equal(98, almostMidnight.DecimalSecond); // 99.98... truncated
    }

    [Fact]
    public void SixAM_Is2Point5DecimalHours()
    {
        var sixAm = new DecDateTime(2024, 1, 1, 6, 0, 0);

        Assert.Equal(2, sixAm.DecimalHour);
        Assert.Equal(50, sixAm.DecimalMinute);
        Assert.Equal(0, sixAm.DecimalSecond);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]      // Midnight
    [InlineData(6, 0, 0, 2, 50, 0)]     // 6 AM
    [InlineData(12, 0, 0, 5, 0, 0)]     // Noon
    [InlineData(18, 0, 0, 7, 50, 0)]    // 6 PM
    public void StandardTimeToDecimalTime_ConversionsAreCorrect(
        int hour, int minute, int second,
        int expectedDecHour, int expectedDecMin, int expectedDecSec)
    {
        var decDateTime = new DecDateTime(2024, 1, 1, hour, minute, second);

        Assert.Equal(expectedDecHour, decDateTime.DecimalHour);
        Assert.Equal(expectedDecMin, decDateTime.DecimalMinute);
        Assert.Equal(expectedDecSec, decDateTime.DecimalSecond);
    }

    #endregion

    #region FromDecimalTime Tests

    [Fact]
    public void FromDecimalTime_Noon_CreatesCorrectTime()
    {
        var decDateTime = DecDateTime.FromDecimalTime(2024, 1, 1, 5, 0, 0);

        Assert.Equal(12, decDateTime.Hour);
        Assert.Equal(0, decDateTime.Minute);
    }

    [Fact]
    public void FromDecimalTime_Midnight_CreatesCorrectTime()
    {
        var decDateTime = DecDateTime.FromDecimalTime(2024, 1, 1, 0, 0, 0);

        Assert.Equal(0, decDateTime.Hour);
        Assert.Equal(0, decDateTime.Minute);
        Assert.Equal(0, decDateTime.Second);
    }

    [Fact]
    public void FromDecimalTime_InvalidDecimalHour_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DecDateTime.FromDecimalTime(2024, 1, 1, 10, 0, 0));
    }

    [Fact]
    public void FromDecimalTime_InvalidDecimalMinute_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DecDateTime.FromDecimalTime(2024, 1, 1, 5, 100, 0));
    }

    [Fact]
    public void FromDecimalTime_InvalidDecimalSecond_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DecDateTime.FromDecimalTime(2024, 1, 1, 5, 0, 100));
    }

    [Fact]
    public void FromDecimalTime_RoundTrip_PreservesDecimalTime()
    {
        var original = DecDateTime.FromDecimalTime(2024, 6, 15, 7, 35, 42);

        Assert.Equal(7, original.DecimalHour);
        Assert.Equal(35, original.DecimalMinute);
        Assert.Equal(42, original.DecimalSecond);
    }

    #endregion

    #region Date Properties Tests

    [Fact]
    public void DateProperties_PassThroughCorrectly()
    {
        var dateTime = new DateTime(2024, 6, 15, 14, 30, 45);
        var decDateTime = new DecDateTime(dateTime);

        Assert.Equal(2024, decDateTime.Year);
        Assert.Equal(6, decDateTime.Month);
        Assert.Equal(15, decDateTime.Day);
        Assert.Equal(DayOfWeek.Saturday, decDateTime.DayOfWeek);
        Assert.Equal(167, decDateTime.DayOfYear);
    }

    [Fact]
    public void Date_ReturnsDateWithMidnight()
    {
        var decDateTime = new DecDateTime(2024, 6, 15, 14, 30, 45);
        var dateOnly = decDateTime.Date;

        Assert.Equal(2024, dateOnly.Year);
        Assert.Equal(6, dateOnly.Month);
        Assert.Equal(15, dateOnly.Day);
        Assert.Equal(0, dateOnly.Hour);
        Assert.Equal(0, dateOnly.DecimalHour);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void Equals_SameDateTime_ReturnsTrue()
    {
        var dt1 = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var dt2 = new DecDateTime(2024, 6, 15, 12, 0, 0);

        Assert.True(dt1.Equals(dt2));
        Assert.True(dt1 == dt2);
        Assert.False(dt1 != dt2);
    }

    [Fact]
    public void Equals_DifferentDateTime_ReturnsFalse()
    {
        var dt1 = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var dt2 = new DecDateTime(2024, 6, 15, 13, 0, 0);

        Assert.False(dt1.Equals(dt2));
        Assert.False(dt1 == dt2);
        Assert.True(dt1 != dt2);
    }

    [Fact]
    public void CompareTo_EarlierDate_ReturnsNegative()
    {
        var earlier = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var later = new DecDateTime(2024, 6, 15, 13, 0, 0);

        Assert.True(earlier.CompareTo(later) < 0);
        Assert.True(earlier < later);
        Assert.True(earlier <= later);
    }

    [Fact]
    public void CompareTo_LaterDate_ReturnsPositive()
    {
        var earlier = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var later = new DecDateTime(2024, 6, 15, 13, 0, 0);

        Assert.True(later.CompareTo(earlier) > 0);
        Assert.True(later > earlier);
        Assert.True(later >= earlier);
    }

    [Fact]
    public void CompareTo_SameDate_ReturnsZero()
    {
        var dt1 = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var dt2 = new DecDateTime(2024, 6, 15, 12, 0, 0);

        Assert.Equal(0, dt1.CompareTo(dt2));
        Assert.True(dt1 <= dt2);
        Assert.True(dt1 >= dt2);
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        var dt1 = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var dt2 = new DecDateTime(2024, 6, 15, 12, 0, 0);

        Assert.Equal(dt1.GetHashCode(), dt2.GetHashCode());
    }

    #endregion

    #region Conversion Operators Tests

    [Fact]
    public void ImplicitOperator_ToDateTime_Works()
    {
        DecDateTime decDateTime = new(2024, 6, 15, 12, 0, 0);
        DateTime dateTime = decDateTime;

        Assert.Equal(new DateTime(2024, 6, 15, 12, 0, 0), dateTime);
    }

    [Fact]
    public void ExplicitOperator_FromDateTime_Works()
    {
        DateTime dateTime = new(2024, 6, 15, 12, 0, 0);
        DecDateTime decDateTime = (DecDateTime)dateTime;

        Assert.Equal(dateTime, decDateTime.ToDateTime());
    }

    #endregion

    #region Arithmetic Operators Tests

    [Fact]
    public void Addition_WithDecTimeSpan_Works()
    {
        var decDateTime = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var timeSpan = DecTimeSpan.FromDecimalHours(2.5); // 2.5 decimal hours = 6 standard hours

        var result = decDateTime + timeSpan;

        Assert.Equal(18, result.Hour);
    }

    [Fact]
    public void Subtraction_WithDecTimeSpan_Works()
    {
        var decDateTime = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var timeSpan = DecTimeSpan.FromDecimalHours(2.5); // 2.5 decimal hours = 6 standard hours

        var result = decDateTime - timeSpan;

        Assert.Equal(6, result.Hour);
    }

    [Fact]
    public void Subtraction_TwoDecDateTimes_ReturnsDecTimeSpan()
    {
        var dt1 = new DecDateTime(2024, 6, 15, 18, 0, 0);
        var dt2 = new DecDateTime(2024, 6, 15, 12, 0, 0);

        DecTimeSpan diff = dt1 - dt2;

        Assert.Equal(2.5, diff.TotalDecimalHours, 1);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToDecimalTimeString_FormatsCorrectly()
    {
        var decDateTime = new DecDateTime(2024, 6, 15, 12, 0, 0); // Noon = 5:00:00 decimal

        Assert.Equal("5:00:00", decDateTime.ToDecimalTimeString());
    }

    [Fact]
    public void ToString_IncludesDateAndDecimalTime()
    {
        var decDateTime = new DecDateTime(2024, 6, 15, 12, 0, 0);
        var result = decDateTime.ToString();

        Assert.Contains("2024-06-15", result);
        Assert.Contains("5:00:00", result);
    }

    #endregion
}
