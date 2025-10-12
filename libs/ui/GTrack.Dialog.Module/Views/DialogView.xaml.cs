using System.Windows;

namespace GTrack.Dialog.Module.Views;

public partial class DialogView : Window, IDialogWindow
{
    public DialogView()
    {
        InitializeComponent();
    }

    public IDialogResult Result { get; set; }
}