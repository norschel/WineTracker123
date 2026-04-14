using System.ComponentModel;
using System.Windows;
using WineTracker.ViewModels;

namespace WineTracker.Views;

public partial class WineEditDialog : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public WineViewModel WineVm { get; }

    private string _showLinkError = string.Empty;
    public string ShowLinkError
    {
        get => _showLinkError;
        private set { _showLinkError = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowLinkError))); }
    }

    public WineEditDialog(WineViewModel vm, string dialogTitle)
    {
        WineVm = vm;
        Title = dialogTitle;
        InitializeComponent();
        DataContext = this;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        WineVm.TriggerValidation();

        if (WineVm.HasErrors)
            return;

        if (!WineVm.IsLinkValid)
        {
            ShowLinkError = "invalid";
            return;
        }

        ShowLinkError = string.Empty;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
