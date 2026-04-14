using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WineTracker.Controls;

public partial class StarRatingControl : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public static readonly DependencyProperty RatingProperty =
        DependencyProperty.Register(
            nameof(Rating),
            typeof(int),
            typeof(StarRatingControl),
            new FrameworkPropertyMetadata(1,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnRatingChanged));

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(
            nameof(IsReadOnly),
            typeof(bool),
            typeof(StarRatingControl),
            new PropertyMetadata(false));

    public int Rating
    {
        get => (int)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public string Star1 => Rating >= 1 ? "★" : "☆";
    public string Star2 => Rating >= 2 ? "★" : "☆";
    public string Star3 => Rating >= 3 ? "★" : "☆";
    public string Star4 => Rating >= 4 ? "★" : "☆";
    public string Star5 => Rating >= 5 ? "★" : "☆";

    public StarRatingControl()
    {
        InitializeComponent();
    }

    private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StarRatingControl ctrl)
            ctrl.RefreshStars();
    }

    private void RefreshStars()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Star1)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Star2)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Star3)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Star4)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Star5)));
    }

    private void Star_Click(object sender, RoutedEventArgs e)
    {
        if (IsReadOnly) return;
        if (sender is Button btn && btn.Tag is string tagStr && int.TryParse(tagStr, out int val))
            Rating = val;
    }
}
