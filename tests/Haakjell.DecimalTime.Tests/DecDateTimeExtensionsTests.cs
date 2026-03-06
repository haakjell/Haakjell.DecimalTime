using Xunit;

namespace Haakjell.DecimalTime.Tests;

public class DecDateTimeExtensionsTests
{
    [Fact]
    public void ToDecDateTime_ConvertsCorrectly()
    {
        var dateTime = new DateTime(2024, 6, 15, 12, 0, 0);

        var result = dateTime.ToDecDateTime();

        Assert.Equal(dateTime, result.ToDateTime());
        Assert.Equal(5, result.DecimalHour);
    }

    [Fact]
    public void ToDecTimeSpan_ConvertsCorrectly()
    {
        var timeSpan = TimeSpan.FromHours(12);

        var result = timeSpan.ToDecTimeSpan();

        Assert.Equal(timeSpan, result.ToTimeSpan());
        Assert.Equal(5, result.TotalDecimalHours, 1);
    }

    [Fact]
    public void ToDecTimeOnly_ConvertsCorrectly()
    {
        var timeOnly = new TimeOnly(12, 0, 0);

        var result = timeOnly.ToDecTimeOnly();

        Assert.Equal(timeOnly, result.ToTimeOnly());
        Assert.Equal(5, result.DecimalHour);
    }
}
