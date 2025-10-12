using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace GTrack.Node.Desktop.Behaviors;

/// <summary>
/// Behavior for restricting TextBox input to numeric values only.
/// Поведение для ограничения ввода в TextBox только числовыми значениями.
/// </summary>
public static class NumericTextBoxBehavior
{
    // ----- Attached Property Getter -----
    // Returns whether the "AllowOnlyNumbers" property is enabled on the element.
    // Возвращает значение свойства "AllowOnlyNumbers" для элемента.
    public static bool GetAllowOnlyNumbers(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowOnlyNumbersProperty);
    }

    // ----- Attached Property Setter -----
    // Sets the "AllowOnlyNumbers" property on the element.
    // Устанавливает значение свойства "AllowOnlyNumbers" для элемента.
    public static void SetAllowOnlyNumbers(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowOnlyNumbersProperty, value);
    }

    // ----- Attached Property Registration -----
    // Registers the "AllowOnlyNumbers" attached property.
    // Регистрирует присоединяемое свойство "AllowOnlyNumbers".
    public static readonly DependencyProperty AllowOnlyNumbersProperty =
        DependencyProperty.RegisterAttached(
            "AllowOnlyNumbers",
            typeof(bool),
            typeof(NumericTextBoxBehavior),
            new PropertyMetadata(false, OnAllowOnlyNumbersChanged));

    // ----- Property Changed Callback -----
    // Called when the "AllowOnlyNumbers" property value changes.
    // Вызывается при изменении значения свойства "AllowOnlyNumbers".
    private static void OnAllowOnlyNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
                textBox.PreviewTextInput += TextBox_PreviewTextInput; // Attach numeric input handler
            else
                textBox.PreviewTextInput -= TextBox_PreviewTextInput; // Detach numeric input handler
        }
    }

    // ----- PreviewTextInput Handler -----
    // Handles TextInput event to allow only numeric characters.
    // Обрабатывает событие TextInput, разрешая только числовые символы.
    private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$"); // Block input if not a number
    }
}