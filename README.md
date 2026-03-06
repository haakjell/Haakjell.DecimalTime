# Haakjell.DecimalTime

A .NET library that provides decimal (metric) time functionality through the `DecDateTime`, `DecTimeSpan`, and `DecTimeOnly` types.

## Decimal Time System

In decimal time:
- 1 day = 10 decimal hours
- 1 decimal hour = 100 decimal minutes
- 1 decimal minute = 100 decimal seconds
- 1 day = 100,000 decimal seconds

This means noon (12:00:00) in standard time equals 5:00:00 in decimal time.

## Installation

```bash
dotnet add package Haakjell.DecimalTime
```

## Usage

### Basic Usage

```csharp
using Haakjell.DecimalTime;

// Get current time in decimal format
var now = DecDateTime.Now;
Console.WriteLine($"Current decimal time: {now.ToDecimalTimeString()}");
// Output: Current decimal time: 7:35:42 (example)

// Create from standard DateTime
var noon = new DecDateTime(2024, 6, 15, 12, 0, 0);
Console.WriteLine($"Noon in decimal: {noon.DecimalHour}:{noon.DecimalMinute:D2}:{noon.DecimalSecond:D2}");
// Output: Noon in decimal: 5:00:00

// Create from decimal time components
var decTime = DecDateTime.FromDecimalTime(2024, 6, 15, 5, 0, 0);
Console.WriteLine($"Standard hour: {decTime.Hour}");
// Output: Standard hour: 12
```

### Working with DecTimeSpan

```csharp
// Create decimal time intervals
var twoAndHalfDecimalHours = DecTimeSpan.FromDecimalHours(2.5);
Console.WriteLine($"In standard hours: {twoAndHalfDecimalHours.ToTimeSpan().TotalHours}");
// Output: In standard hours: 6

// Arithmetic with DecDateTime
var later = DecDateTime.Now + DecTimeSpan.FromDecimalHours(1);

// Difference between times
var diff = later - DecDateTime.Now;
Console.WriteLine($"Difference: {diff.TotalDecimalHours} decimal hours");
```

### Working with DecTimeOnly

```csharp
// Time-of-day without date component
var lunchTime = new DecTimeOnly(12, 0, 0); // Noon
Console.WriteLine($"Lunch in decimal: {lunchTime.ToDecimalTimeString()}");
// Output: Lunch in decimal: 5:00:00

// Create from decimal time
var meetingTime = DecTimeOnly.FromDecimalTime(7, 50, 0); // 6 PM
Console.WriteLine($"Meeting at: {meetingTime.Hour}:00");
// Output: Meeting at: 18:00

// Use extension method
var now = TimeOnly.FromDateTime(DateTime.Now).ToDecTimeOnly();
```

### Properties

`DecDateTime` provides both standard and decimal time properties:

```csharp
var dt = DecDateTime.Now;

// Standard properties (pass-through from DateTime)
int year = dt.Year;
int month = dt.Month;
int day = dt.Day;
int hour = dt.Hour;           // 0-23
int minute = dt.Minute;       // 0-59
int second = dt.Second;       // 0-59

// Decimal time properties
int decHour = dt.DecimalHour;     // 0-9
int decMin = dt.DecimalMinute;    // 0-99
int decSec = dt.DecimalSecond;    // 0-99
double totalDecSec = dt.TotalDecimalSeconds;  // 0-99999.999...
```

### Conversion

```csharp
// Convert to/from DateTime
DateTime standardTime = decDateTime.ToDateTime();
DecDateTime decTime = DecDateTime.FromDateTime(standardTime);

// Implicit/explicit conversion operators
DateTime dt = decDateTime;                    // implicit to DateTime
DecDateTime ddt = (DecDateTime)dateTime;      // explicit from DateTime
```

## API Reference

### DecDateTime

| Member | Description |
|--------|-------------|
| `Now`, `UtcNow`, `Today` | Static properties for current time |
| `MinValue`, `MaxValue` | Range boundaries |
| `FromDateTime(DateTime)` | Create from standard DateTime |
| `FromDecimalTime(...)` | Create from decimal time components |
| `DecimalHour`, `DecimalMinute`, `DecimalSecond` | Decimal time components |
| `TotalDecimalSeconds` | Total decimal seconds since midnight |
| `ToDateTime()` | Convert to standard DateTime |
| `ToDecimalTimeString()` | Format as "h:mm:ss" decimal time |

### DecTimeSpan

| Member | Description |
|--------|-------------|
| `Zero`, `MinValue`, `MaxValue` | Static properties |
| `FromTimeSpan(TimeSpan)` | Create from standard TimeSpan |
| `FromDecimalHours(double)` | Create from decimal hours |
| `FromDecimalMinutes(double)` | Create from decimal minutes |
| `FromDecimalSeconds(double)` | Create from decimal seconds |
| `TotalDecimalHours`, `TotalDecimalMinutes`, `TotalDecimalSeconds` | Total values |
| `ToTimeSpan()` | Convert to standard TimeSpan |

### DecTimeOnly

| Member | Description |
|--------|-------------|
| `Midnight`, `Noon`, `MinValue`, `MaxValue` | Static properties for common times |
| `FromTimeOnly(TimeOnly)` | Create from standard TimeOnly |
| `FromDecimalTime(...)` | Create from decimal time components |
| `DecimalHour`, `DecimalMinute`, `DecimalSecond` | Decimal time components |
| `TotalDecimalSeconds` | Total decimal seconds since midnight |
| `ToTimeOnly()` | Convert to standard TimeOnly |
| `ToDecimalTimeString()` | Format as "h:mm:ss" decimal time |

## Future Goals

This library aims to achieve full API parity with `System.DateTime` and `System.TimeSpan`. Future versions will include:

- Parsing methods (`Parse`, `TryParse`)
- Additional formatting options
- Date arithmetic methods (`AddDays`, `AddDecimalHours`, etc.)
- Time zone support
- Calendar integrations

## License

MIT
