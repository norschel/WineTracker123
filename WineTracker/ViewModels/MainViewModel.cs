using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WineTracker.Data;
using WineTracker.Models;

namespace WineTracker.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly WineDbContext _db;

    public ObservableCollection<WineViewModel> Wines { get; } = new();

    public ICollectionView FilteredWines { get; }

    [ObservableProperty]
    private WineViewModel? _selectedWine;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _sortByRatingDescending;

    public MainViewModel(WineDbContext db)
    {
        _db = db;

        FilteredWines = CollectionViewSource.GetDefaultView(Wines);
        FilteredWines.Filter = FilterWine;
        FilteredWines.SortDescriptions.Add(new SortDescription(nameof(WineViewModel.Name), ListSortDirection.Ascending));
    }

    partial void OnSearchTextChanged(string value) => FilteredWines.Refresh();

    partial void OnSortByRatingDescendingChanged(bool value)
    {
        FilteredWines.SortDescriptions.Clear();
        if (value)
            FilteredWines.SortDescriptions.Add(new SortDescription(nameof(WineViewModel.Rating), ListSortDirection.Descending));
        else
            FilteredWines.SortDescriptions.Add(new SortDescription(nameof(WineViewModel.Name), ListSortDirection.Ascending));
    }

    private bool FilterWine(object obj)
    {
        if (obj is not WineViewModel vm) return false;
        if (string.IsNullOrWhiteSpace(SearchText)) return true;
        var q = SearchText.Trim();
        return vm.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
            || vm.Winery.Contains(q, StringComparison.OrdinalIgnoreCase)
            || vm.City.Contains(q, StringComparison.OrdinalIgnoreCase);
    }

    public void LoadWines()
    {
        try
        {
            _db.Database.EnsureCreated();
            var wines = _db.Wines.AsNoTracking().OrderBy(w => w.Name).ToList();
            Wines.Clear();
            foreach (var w in wines)
                Wines.Add(new WineViewModel(w));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load wines:\n{ex.Message}", "Database Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void ClearSearch() => SearchText = string.Empty;

    [RelayCommand]
    private void AddWine()
    {
        var vm = new WineViewModel();
        var result = ShowEditDialog(vm, "Add Wine");
        if (!result) return;

        try
        {
            var wine = vm.ToModel();
            wine.CreatedAt = DateTime.UtcNow;
            _db.Wines.Add(wine);
            _db.SaveChanges();

            var saved = new WineViewModel(wine);
            Wines.Add(saved);
            SelectedWine = saved;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save wine:\n{ex.Message}", "Database Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedWine))]
    private void EditWine()
    {
        if (SelectedWine is null) return;

        // Clone values into a temporary VM for editing
        var edit = new WineViewModel(SelectedWine.ToModel());
        var result = ShowEditDialog(edit, "Edit Wine");
        if (!result) return;

        try
        {
            var tracked = _db.Wines.Find(edit.Id);
            if (tracked is null) return;

            tracked.Name = edit.Name;
            tracked.Description = edit.Description;
            tracked.Rating = edit.Rating;
            tracked.RatingDescription = edit.RatingDescription;
            tracked.Winery = edit.Winery;
            tracked.City = edit.City;
            tracked.Link = edit.Link;
            _db.SaveChanges();

            // Refresh in the list
            var idx = Wines.IndexOf(SelectedWine);
            Wines[idx] = new WineViewModel(tracked);
            SelectedWine = Wines[idx];
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update wine:\n{ex.Message}", "Database Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedWine))]
    private void DeleteWine()
    {
        if (SelectedWine is null) return;

        var confirm = MessageBox.Show(
            $"Delete \"{SelectedWine.Name}\"?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            var tracked = _db.Wines.Find(SelectedWine.Id);
            if (tracked is not null)
            {
                _db.Wines.Remove(tracked);
                _db.SaveChanges();
            }

            Wines.Remove(SelectedWine);
            SelectedWine = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete wine:\n{ex.Message}", "Database Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenLink))]
    private void OpenLink()
    {
        if (SelectedWine is null || string.IsNullOrWhiteSpace(SelectedWine.Link)) return;

        try
        {
            Process.Start(new ProcessStartInfo(SelectedWine.Link) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open link:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    partial void OnSelectedWineChanged(WineViewModel? value)
    {
        EditWineCommand.NotifyCanExecuteChanged();
        DeleteWineCommand.NotifyCanExecuteChanged();
        OpenLinkCommand.NotifyCanExecuteChanged();
    }

    private bool HasSelectedWine() => SelectedWine is not null;
    private bool CanOpenLink() => SelectedWine is not null && !string.IsNullOrWhiteSpace(SelectedWine.Link);

    /// <summary>
    /// Opens the edit dialog. The actual dialog creation is injected via a
    /// delegate so the ViewModel remains testable without a real Window.
    /// </summary>
    public Func<WineViewModel, string, bool>? ShowEditDialogFunc { get; set; }

    private bool ShowEditDialog(WineViewModel vm, string title) =>
        ShowEditDialogFunc?.Invoke(vm, title) ?? false;
}
