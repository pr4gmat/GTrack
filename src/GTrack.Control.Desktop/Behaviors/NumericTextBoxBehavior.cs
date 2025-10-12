using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace GTrack.Control.Desktop.Behaviors;

/// <summary>
/// Attached behavior to allow only numeric input in TextBox.
/// / Поведение для TextBox, разрешающее ввод только чисел.
/// </summary>
public static class NumericTextBoxBehavior
{
    /// <summary>
    /// Gets whether only numeric input is allowed.
    /// / Получает флаг, разрешающий только числа.
    /// </summary>
    public static bool GetAllowOnlyNumbers(DependencyObject obj)
        => (bool)obj.GetValue(AllowOnlyNumbersProperty);

    /// <summary>
    /// Sets whether only numeric input is allowed.
    /// / Устанавливает флаг, разрешающий только числа.
    /// </summary>
    public static void SetAllowOnlyNumbers(DependencyObject obj, bool value)
        => obj.SetValue(AllowOnlyNumbersProperty, value);

    /// <summary>
    /// Attached property to enable numeric-only input.
    /// / Прикрепляемое свойство для ввода только чисел.
    /// </summary>
    public static readonly DependencyProperty AllowOnlyNumbersProperty =
        DependencyProperty.RegisterAttached(
            "AllowOnlyNumbers",
            typeof(bool),
            typeof(NumericTextBoxBehavior),
            new PropertyMetadata(false, OnAllowOnlyNumbersChanged));

    private static void OnAllowOnlyNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
            else
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
        }
    }

    private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    }
}