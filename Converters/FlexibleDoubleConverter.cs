using System.Globalization;

namespace EvidencijaStudenata.Converters;

/// <summary>
/// Osigurava predvidljivu pretvorbu teksta u decimalni broj za polje
/// Prosjek. Ugrađeni MAUI pretvarač oslanja se na trenutačne regionalne
/// postavke uređaja i zna nepredvidivo zakazati na nepotpun unos (npr. "3,"
/// dok korisnik još tipka decimalni broj), zbog čega su gumbi Dodaj/Spremi
/// izmjene znali ostati zaključani iako je unos ispravan - vidi poglavlje
/// 4.7. rada.
/// </summary>
public class FlexibleDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
            return d.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var text = (value as string)?.Replace(',', '.');
        if (string.IsNullOrWhiteSpace(text))
            return 0d;

        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0d;
    }
}