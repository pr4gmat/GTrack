using System.Globalization;
using System.Windows.Data;
using GTrack.Core.Converters;

namespace GTrack.Control.Desktop.Converters;

/// <summary>
/// Converts a boolean value to a status string using ToStatus extension.
/// / Преобразует булево значение в строку статуса с помощью расширения ToStatus.
/// </summary>
public class BooleanToStatusConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean to a status string.
    /// / Преобразует булево значение в строку статуса.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? b.ToStatus() : "Не определено";

    /// <summary>
    /// ConvertBack is not implemented.
    /// / Обратное преобразование не реализовано.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}