using System;
using System.Globalization;

namespace Beamable.StellarFederation.Features.Common;

public readonly record struct StellarAmount
{
    private const int NativeDecimals = 7;
    private long TotalUnits { get; }
    private int Decimals { get; }

    public static readonly StellarAmount NativeZero = new(0);

    public StellarAmount(long totalUnits, int decimals = NativeDecimals)
    {
        if (decimals is < 0 or > 18)
            throw new ArgumentOutOfRangeException(nameof(decimals), "Decimals must be between 0 and 18.");

        TotalUnits = totalUnits;
        Decimals = decimals;
    }

    public static StellarAmount Parse(string amount, int decimals = NativeDecimals)
    {
        if (amount is null)
            throw new ArgumentNullException(nameof(amount));

        if (!decimal.TryParse(amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalAmount))
            throw new FormatException($"Invalid amount format: '{amount}'");

        return FromDecimal(decimalAmount, decimals);
    }

    public static StellarAmount FromDecimal(decimal amount, int decimals = NativeDecimals)
    {
        if (decimals is < 0 or > 18)
            throw new ArgumentOutOfRangeException(nameof(decimals), "Decimals must be between 0 and 18.");

        var factor = (decimal)Math.Pow(10, decimals);
        var totalUnits = (long)Math.Round(amount * factor);
        return new StellarAmount(totalUnits, decimals);
    }

    public static StellarAmount FromLong(long amount, int decimals = NativeDecimals)
    {
        if (decimals is < 0 or > 18)
            throw new ArgumentOutOfRangeException(nameof(decimals), "Decimals must be between 0 and 18.");

        var factor = (decimal)Math.Pow(10, decimals);
        var totalUnits = (long)Math.Round(amount * factor);
        return new StellarAmount(totalUnits, decimals);
    }

    public decimal ToXlm()
    {
        if (Decimals == 0) return TotalUnits;
        var factor = (decimal)Math.Pow(10, Decimals);
        return TotalUnits / factor;
    }

    public string ToXlmString()
    {
        var value = ToXlm();
        var format = "F" + Decimals.ToString(CultureInfo.InvariantCulture);
        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        return ToXlmString();
    }

    private static void EnsureMatchingDecimals(StellarAmount a, StellarAmount b)
    {
        if (a.Decimals != b.Decimals)
        {
            throw new InvalidOperationException(
                $"Cannot perform arithmetic on amounts with different decimal precisions ({a.Decimals} and {b.Decimals}).");
        }
    }

    public static implicit operator long(StellarAmount amount) => amount.TotalUnits;

    public static implicit operator StellarAmount(long stroops) =>
        new (stroops, NativeDecimals);

    public static StellarAmount operator +(StellarAmount a, StellarAmount b)
    {
        EnsureMatchingDecimals(a, b);
        return new StellarAmount(checked(a.TotalUnits + b.TotalUnits), a.Decimals);
    }

    public static StellarAmount operator +(StellarAmount a, long b)
    {
        return new StellarAmount(checked(a.TotalUnits + b), a.Decimals);
    }

    public static StellarAmount operator -(StellarAmount a, StellarAmount b)
    {
        EnsureMatchingDecimals(a, b);
        return new StellarAmount(checked(a.TotalUnits - b.TotalUnits), a.Decimals);
    }
}