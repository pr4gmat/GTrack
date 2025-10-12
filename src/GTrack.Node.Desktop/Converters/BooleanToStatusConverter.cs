using System.Globalization;
using System.Windows.Data;
using GTrack.Core.Converters;

namespace GTrack.Node.Desktop.Converters;

/// <summary>
/// Converter that converts a boolean value to a status string.
/// Конвертер, который преобразует булевое значение в строку статуса.
/// </summary>
public class BooleanToStatusConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a human-readable status string.
    /// Преобразует булевое значение в читаемую строку статуса.
    /// </summary>
    /// <param name="value">Input value (expected bool).</param>
    /// <param name="targetType">The target type (usually string).</param>
    /// <param name="parameter">Optional parameter (unused).</param>
    /// <param name="culture">Culture info for formatting.</param>
    /// <returns>Status string ("Active"/"Inactive" or localized string).</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? b.ToStatus() : "Не определено"; // If not bool, return "Undefined"
    }

    /// <summary>
    /// ConvertBack is not implemented since this is a one-way converter.
    /// Обратное преобразование не реализовано, так как это конвертер только в одну сторону.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}