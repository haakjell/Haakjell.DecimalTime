using Xunit;

namespace Haakjell.DecimalTime.Tests;

public class DecTimeSpanTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_FromTimeSpan_PreservesValue()
    {
        var timeSpan = TimeSpan.FromHours(12);
        var decTimeSpan = new DecTimeSpan(timeSpan);

        Assert.Equal(timeSpan, decTimeSpan.ToTimeSpan());
    }

    [Fact]
    public void Constructor_FromDecimalComponents_CreatesCorrectTimeSpan()
    {
        // 1 day, 5 decimal hours, 0 decimal minutes, 0 decimal seconds
        // 5 decimal hours = 12 standard hours
        var decTimeSpan = new DecTimeSpan(1, 5, 0, 0);

        Assert.Equal(1, decTimeSpan.Days);
        Assert.Equal(5, decTimeSpan.DecimalHours);
        Assert.Equal(36, decTimeSpan.ToTimeSpan().TotalHours, 1); // 24 + 12 = 36 hours total
    }

    #endregion

    #region Static Properties Tests

    [Fact]
    public void Zero_IsZeroTimeSpan()
    {
        Assert.Equal(TimeSpan.Zero, DecTimeSpan.Zero.ToTimeSpan());
    }

    [Fact]
    public void MinValue_MatchesTimeSpanMinValue()
    {
        Assert.Equal(TimeSpan.MinValue, DecTimeSpan.MinValue.ToTimeSpan());
    }

    [Fact]
    public void MaxValue_MatchesTimeSpanMaxValue()
    {
        Assert.Equal(TimeSpan.MaxValue, DecTimeSpan.MaxValue.ToTimeSpan());
    }

    #endregion

    #region Factory Methods Tests

    [Fact]
    public void FromTimeSpan_CreatesEquivalentDecTimeSpan()
    {
        var timeSpan = TimeSpan.FromHours(6);
        var decTimeSpan = DecTimeSpan.FromTimeSpan(timeSpan);

        Assert.Equal(timeSpan, decTimeSpan.ToTimeSpan());
    }

    [Fact]
    public void FromDecimalHours_CreatesCorrectTimeSpan()
    {
        // 5 decimal hours = half a day = 12 standard hours
        var decTimeSpan = DecTimeSpan.FromDecimalHours(5);

        Assert.Equal(12, decTimeSpan.ToTimeSpan().TotalHours, 1);
    }

    [Fact]
    public void FromDecimalMinutes_CreatesCorrectTimeSpan()
    {
        // 500 decimal minutes = 5 decimal hours = 12 standard hours
        var decTimeSpan = DecTimeSpan.FromDecimalMinutes(500);

        Assert.Equal(12, decTimeSpan.ToTimeSpan().TotalHours, 1);
    }

    [Fact]
    public void FromDecimalSeconds_CreatesCorrectTimeSpan()
    {
        // 50000 decimal seconds = 5 decimal hours = 12 standard hours
        var decTimeSpan = DecTimeSpan.FromDecimalSeconds(50000);

        Assert.Equal(12, decTimeSpan.ToTimeSpan().TotalHours, 1);
    }

    #endregion

    #region Component Properties Tests

    [Fact]
    public void DecimalHours_ReturnsCorrectComponent()
    {
        // 12 standard hours = 5 decimal hours
        var decTimeSpan = new DecTimeSpan(TimeSpan.FromHours(12));

        Assert.Equal(5, decTimeSpan.DecimalHours);
    }

    [Fact]
    public void DecimalMinutes_ReturnsCorrectComponent()
    {
        // 12 hours + 1.44 minutes = 5 decimal hours + 1 decimal minute
        // 1.44 standard minutes = 86.4 standard seconds = 100 decimal seconds = 1 decimal minute
        var decTimeSpan = new DecTimeSpan(TimeSpan.FromHours(12).Add(TimeSpan.FromSeconds(86.4)));

        Assert.Equal(5, decTimeSpan.DecimalHours);
        Assert.Equal(1, decTimeSpan.DecimalMinutes);
    }

    [Fact]
    public void Days_ReturnsCorrectComponent()
    {
        var decTimeSpan = new DecTimeSpan(TimeSpan.FromDays(3));

        Assert.Equal(3, decTimeSpan.Days);
    }

    #endregion

    #region Total Properties Tests

    [Fact]
    public void TotalDays_ReturnsCorrectValue()
    {
        var decTimeSpan = new DecTimeSpan(TimeSpan.FromDays(2.5));

        Assert.Equal(2.5, decTimeSpan.TotalDays, 1);
    }

    [Fact]
    public void TotalDecimalHours_ReturnsCorrectValue()
    {
        // 1 day = 10 decimal hours
        var decTimeSpan = new DecTimeSpan(TimeSpan.FromDays(1));

        Assert.Equal(10, decTimeSpan.TotalDecimalHours, 1);
    }

    [Fact]
    public void TotalDecimalMinutes_ReturnsCorrectValue()
    {
        // 1 day = 1000 decimal minutes
        var decTimeSpan = new DecTimeSpan(TimeSpan.FromDays(1));

        Assert.Equal(1000, decTimeSpan.TotalDecimalMinutes, 1);
    }

    [Fact]
    public void TotalDecimalSeconds_ReturnsCorrectValue()
    {
        // 1 day = 100000 decimal seconds
        var decTimeSpan = new DecTimeSpan(TimeSpan.FromDays(1));

        Assert.Equal(100000, decTimeSpan.TotalDecimalSeconds, 1);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void Equals_SameTimeSpan_ReturnsTrue()
    {
        var ts1 = DecTimeSpan.FromDecimalHours(5);
        var ts2 = DecTimeSpan.FromDecimalHours(5);

        Assert.True(ts1.Equals(ts2));
        Assert.True(ts1 == ts2);
        Assert.False(ts1 != ts2);
    }

    [Fact]
    public void Equals_DifferentTimeSpan_ReturnsFalse()
    {
        var ts1 = DecTimeSpan.FromDecimalHours(5);
        var ts2 = DecTimeSpan.FromDecimalHours(6);

        Assert.False(ts1.Equals(ts2));
        Assert.False(ts1 == ts2);
        Assert.True(ts1 != ts2);
    }

    [Fact]
    public void CompareTo_ShorterDuration_ReturnsNegative()
    {
        var shorter = DecTimeSpan.FromDecimalHours(3);
        var longer = DecTimeSpan.FromDecimalHours(5);

        Assert.True(shorter.CompareTo(longer) < 0);
        Assert.True(shorter < longer);
        Assert.True(shorter <= longer);
    }

    [Fact]
    public void CompareTo_LongerDuration_ReturnsPositive()
    {
        var shorter = DecTimeSpan.FromDecimalHours(3);
        var longer = DecTimeSpan.FromDecimalHours(5);

        Assert.True(longer.CompareTo(shorter) > 0);
        Assert.True(longer > shorter);
        Assert.True(longer >= shorter);
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        var ts1 = DecTimeSpan.FromDecimalHours(5);
        var ts2 = DecTimeSpan.FromDecimalHours(5);

        Assert.Equal(ts1.GetHashCode(), ts2.GetHashCode());
    }

    #endregion

    #region Arithmetic Operators Tests

    [Fact]
    public void Addition_TwoDecTimeSpans_Works()
    {
        var ts1 = DecTimeSpan.FromDecimalHours(3);
        var ts2 = DecTimeSpan.FromDecimalHours(2);

        var result = ts1 + ts2;

        Assert.Equal(5, result.TotalDecimalHours, 1);
    }

    [Fact]
    public void Subtraction_TwoDecTimeSpans_Works()
    {
        var ts1 = DecTimeSpan.FromDecimalHours(5);
        var ts2 = DecTimeSpan.FromDecimalHours(2);

        var result = ts1 - ts2;

        Assert.Equal(3, result.TotalDecimalHours, 1);
    }

    [Fact]
    public void Negation_Works()
    {
        var ts = DecTimeSpan.FromDecimalHours(5);

        var negated = -ts;

        Assert.Equal(-5, negated.TotalDecimalHours, 1);
    }

    [Fact]
    public void UnaryPlus_ReturnsSameValue()
    {
        var ts = DecTimeSpan.FromDecimalHours(5);

        var result = +ts;

        Assert.Equal(ts, result);
    }

    #endregion

    #region Conversion Operators Tests

    [Fact]
    public void ImplicitOperator_ToTimeSpan_Works()
    {
        DecTimeSpan decTimeSpan = DecTimeSpan.FromDecimalHours(5);
        TimeSpan timeSpan = decTimeSpan;

        Assert.Equal(TimeSpan.FromHours(12), timeSpan);
    }

    [Fact]
    public void ExplicitOperator_FromTimeSpan_Works()
    {
        TimeSpan timeSpan = TimeSpan.FromHours(12);
        DecTimeSpan decTimeSpan = (DecTimeSpan)timeSpan;

        Assert.Equal(timeSpan, decTimeSpan.ToTimeSpan());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var decTimeSpan = new DecTimeSpan(2, 5, 30, 15);
        var result = decTimeSpan.ToString();

        Assert.Equal("2.5:30:15", result);
    }

    [Fact]
    public void ToString_NegativeValue_IncludesSign()
    {
        var decTimeSpan = -DecTimeSpan.FromDecimalHours(5);
        var result = decTimeSpan.ToString();

        Assert.StartsWith("-", result);
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void RoundTrip_TimeSpanToDecTimeSpanAndBack_PreservesValue()
    {
        var original = TimeSpan.FromHours(15.5);
        var decTimeSpan = DecTimeSpan.FromTimeSpan(original);
        var roundTrip = decTimeSpan.ToTimeSpan();

        Assert.Equal(original.TotalSeconds, roundTrip.TotalSeconds, 0);
    }

    [Fact]
    public void RoundTrip_DecimalHours_PreservesValue()
    {
        var decTimeSpan = DecTimeSpan.FromDecimalHours(7.5);

        Assert.Equal(7.5, decTimeSpan.TotalDecimalHours, 5);
    }

    #endregion
}
