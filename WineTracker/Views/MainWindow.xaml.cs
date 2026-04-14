using System.Windows;
using WineTracker.Data;
using WineTracker.ViewModels;
using WineTracker.Views;

namespace WineTracker.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var db = new WineDbContext();
        var vm = new MainViewModel(db);

        vm.ShowEditDialogFunc = (wineVm, title) =>
        {
            var dialog = new WineEditDialog(wineVm, title) { Owner = this };
            return dialog.ShowDialog() == true;
        };

        DataContext = vm;
        vm.LoadWines();
    }
}
