using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Imgdup.Core.Models;

namespace Imgdup.App.Converters;

/// <summary>Returns true when the bound value equals the converter parameter. Used for enum-bound radio buttons.</summary>
public sealed class EnumEqualsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value?.ToString() == parameter?.ToString();

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Enum.Parse(targetType, parameter?.ToString() ?? string.Empty) : Binding.DoNothing;
}

/// <summary>Maps a duplicate match kind to an accent brush for the group header.</summary>
public sealed class MatchKindToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Exact = new(Color.FromRgb(0xD9, 0x53, 0x4F)); // red-ish
    private static readonly SolidColorBrush Near = new(Color.FromRgb(0x3B, 0x82, 0xF6));  // blue

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is MatchKind.Exact ? Exact : Near;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
