
using System.Globalization;

namespace stefc.mosmix;

internal static class ValueConverter
{
    internal delegate bool TryValueConverter(string xmlValue, out object value);

    private static readonly IDictionary<Type, TryValueConverter> converterLookup
        = new Dictionary<Type, TryValueConverter>
    {
            { typeof(string), ConvertString },
            { typeof(int), ConvertInt },
            { typeof(double), ConvertDouble},
            { typeof(DateTime), ConvertDate},
    };

    internal static TryValueConverter For(Type type)
    {
        if (converterLookup.TryGetValue(type, out var converter))
            return converter;
        throw new NotSupportedException($"No {nameof(ValueConverter)} for type '{type.Name}' present.");
    }

    private static bool ConvertString(string input, out object value)
    {
        value = input?.Trim() ?? string.Empty;
        return true;
    }

    private static bool ConvertInt(string input, out object value)
    {
        var result = ConvertDouble(input, out value);
        value = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        return result;
    }
    
    private static bool ConvertDouble(string input, out object value)
    {
        value = default(double);
        if (string.IsNullOrWhiteSpace(input))
            return true;

        var numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
        var parseResult = double.TryParse(input, numberStyle, CultureInfo.InvariantCulture, out var tmp);
        value = tmp;
        return parseResult;
    }

    // https://mindthe.net/devices/2008/05/23/tryparseexact-and-utc-datetime/
    private static bool ConvertDate(string input, out object value)
    {
        var parseResult = DateTime.TryParseExact(input?.Trim(), "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var tmp);
        value = tmp;
        return parseResult;
    }

    internal static DateTime? ToDate(string input) => (!ConvertDate(input, out var value)) ? null : (DateTime)value;

    internal static DateTime ToDateExact(string input) 
    => DateTime.ParseExact(input.Trim(), "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    internal static decimal ToDecimal(string input) {
        var numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
        return decimal.Parse(input, numberStyle, CultureInfo.InvariantCulture);
    }
}
