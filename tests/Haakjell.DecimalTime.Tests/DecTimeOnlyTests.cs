using Xunit;

namespace Haakjell.DecimalTime.Tests;

public class DecTimeOnlyTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_FromTimeOnly_PreservesValue()
    {
        var timeOnly = new TimeOnly(12, 30, 45);
        var decTimeOnly = new DecTimeOnly(timeOnly);

        Assert.Equal(timeOnly, decTimeOnly.ToTimeOnly());
    }

    [Fact]
    public void Constructor_FromComponents_CreatesCorrectTime()
    {
        var decTimeOnly = new DecTimeOnly(14, 30, 45);

        Assert.Equal(14, decTimeOnly.Hour);
        Assert.Equal(30, decTimeOnly.Minute);
        Assert.Equal(45, decTimeOnly.Second);
    }

    #endregion

    #region Static Properties Tests

    [Fact]
    public void Midnight_IsZeroDecimalTime()
    {
        var midnight = DecTimeOnly.Midnight;

        Assert.Equal(0, midnight.DecimalHour);
        Assert.Equal(0, midnight.DecimalMinute);
        Assert.Equal(0, midnight.DecimalSecond);
    }

    [Fact]
    public void Noon_IsFiveDecimalHours()
    {
        var noon = DecTimeOnly.Noon;

        Assert.Equal(5, noon.DecimalHour);
        Assert.Equal(0, noon.DecimalMinute);
        Assert.Equal(0, noon.DecimalSecond);
    }

    [Fact]
    public void MinValue_MatchesTimeOnlyMinValue()
    {
        Assert.Equal(TimeOnly.MinValue, DecTimeOnly.MinValue.ToTimeOnly());
    }

    [Fact]
    public void MaxValue_MatchesTimeOnlyMaxValue()
    {
        Assert.Equal(TimeOnly.MaxValue, DecTimeOnly.MaxValue.ToTimeOnly());
    }

    #endregion

    #region Decimal Time Conversion Tests

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]      // Midnight
    [InlineData(6, 0, 0, 2, 50, 0)]     // 6 AM
    [InlineData(12, 0, 0, 5, 0, 0)]     // Noon
    [InlineData(18, 0, 0, 7, 50, 0)]    // 6 PM
    public void StandardTimeToDecimalTime_ConversionsAreCorrect(
        int hour, int minute, int second,
        int expectedDecHour, int expectedDecMin, int expectedDecSec)
    {
        var decTimeOnly = new DecTimeOnly(hour, minute, second);

        Assert.Equal(expectedDecHour, decTimeOnly.DecimalHour);
        Assert.Equal(expectedDecMin, decTimeOnly.DecimalMinute);
        Assert.Equal(expectedDecSec, decTimeOnly.DecimalSecond);
    }

    #endregion

    #region FromDecimalTime Tests

    [Fact]
    public void FromDecimalTime_Noon_CreatesCorrectTime()
    {
        var decTimeOnly = DecTimeOnly.FromDecimalTime(5, 0, 0);

        Assert.Equal(12, decTimeOnly.Hour);
        Assert.Equal(0, decTimeOnly.Minute);
    }

    [Fact]
    public void FromDecimalTime_Midnight_CreatesCorrectTime()
    {
        var decTimeOnly = DecTimeOnly.FromDecimalTime(0, 0, 0);

        Assert.Equal(0, decTimeOnly.Hour);
        Assert.Equal(0, decTimeOnly.Minute);
        Assert.Equal(0, decTimeOnly.Second);
    }

    [Fact]
    public void FromDecimalTime_InvalidDecimalHour_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DecTimeOnly.FromDecimalTime(10, 0, 0));
    }

    [Fact]
    public void FromDecimalTime_RoundTrip_PreservesDecimalTime()
    {
        var original = DecTimeOnly.FromDecimalTime(7, 35, 42);

        Assert.Equal(7, original.DecimalHour);
        Assert.Equal(35, original.DecimalMinute);
        Assert.Equal(42, original.DecimalSecond);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void Equals_SameTime_ReturnsTrue()
    {
        var t1 = new DecTimeOnly(12, 30, 0);
        var t2 = new DecTimeOnly(12, 30, 0);

        Assert.True(t1.Equals(t2));
        Assert.True(t1 == t2);
    }

    [Fact]
    public void CompareTo_EarlierTime_ReturnsNegative()
    {
        var earlier = new DecTimeOnly(10, 0, 0);
        var later = new DecTimeOnly(14, 0, 0);

        Assert.True(earlier.CompareTo(later) < 0);
        Assert.True(earlier < later);
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        var t1 = new DecTimeOnly(12, 30, 0);
        var t2 = new DecTimeOnly(12, 30, 0);

        Assert.Equal(t1.GetHashCode(), t2.GetHashCode());
    }

    #endregion

    #region Conversion Operators Tests

    [Fact]
    public void ImplicitOperator_ToTimeOnly_Works()
    {
        DecTimeOnly decTimeOnly = new(12, 30, 0);
        TimeOnly timeOnly = decTimeOnly;

        Assert.Equal(new TimeOnly(12, 30, 0), timeOnly);
    }

    [Fact]
    public void ExplicitOperator_FromTimeOnly_Works()
    {
        TimeOnly timeOnly = new(12, 30, 0);
        DecTimeOnly decTimeOnly = (DecTimeOnly)timeOnly;

        Assert.Equal(timeOnly, decTimeOnly.ToTimeOnly());
    }

    #endregion

    #region Arithmetic Operators Tests

    [Fact]
    public void Addition_WithDecTimeSpan_Works()
    {
        var decTimeOnly = new DecTimeOnly(12, 0, 0);
        var timeSpan = DecTimeSpan.FromDecimalHours(2.5); // 6 standard hours

        var result = decTimeOnly + timeSpan;

        Assert.Equal(18, result.Hour);
    }

    [Fact]
    public void Subtraction_WithDecTimeSpan_Works()
    {
        var decTimeOnly = new DecTimeOnly(12, 0, 0);
        var timeSpan = DecTimeSpan.FromDecimalHours(2.5); // 6 standard hours

        var result = decTimeOnly - timeSpan;

        Assert.Equal(6, result.Hour);
    }

    [Fact]
    public void Subtraction_TwoDecTimeOnlys_ReturnsDecTimeSpan()
    {
        var t1 = new DecTimeOnly(18, 0, 0);
        var t2 = new DecTimeOnly(12, 0, 0);

        DecTimeSpan diff = t1 - t2;

        Assert.Equal(2.5, diff.TotalDecimalHours, 1);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToDecimalTimeString_FormatsCorrectly()
    {
        var decTimeOnly = new DecTimeOnly(12, 0, 0); // Noon = 5:00:00 decimal

        Assert.Equal("5:00:00", decTimeOnly.ToDecimalTimeString());
    }

    [Fact]
    public void ToString_ReturnsDecimalTimeString()
    {
        var decTimeOnly = new DecTimeOnly(12, 0, 0);

        Assert.Equal("5:00:00", decTimeOnly.ToString());
    }

    #endregion
}
