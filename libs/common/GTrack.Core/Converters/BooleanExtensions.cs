namespace GTrack.Core.Converters
{
    /// <summary>
    /// Provides extension methods for the Boolean type.
    /// Converts boolean values into human-readable connection status strings.
    /// </summary>
    /// 
    /// <summary>
    /// Предоставляет методы расширения для типа Boolean.
    /// Преобразует логические значения в удобочитаемые строки состояния подключения.
    /// </summary>
    public static class BooleanExtensions
    {
        /// <summary>
        /// Converts a boolean value to a connection status string.
        /// Returns "Подключено" if true, otherwise "Не подключено".
        /// </summary>
        /// 
        /// <summary>
        /// Преобразует логическое значение в строку состояния подключения.
        /// Возвращает "Подключено", если значение true, иначе — "Не подключено".
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <returns>A string representing connection status.</returns>
        /// 
        /// <param name="value">Логическое значение для преобразования.</param>
        /// <returns>Строка, представляющая состояние подключения.</returns>
        public static string ToStatus(this bool value) => value ? "Подключено" : "Не подключено";
    }
}