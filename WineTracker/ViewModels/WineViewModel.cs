using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using WineTracker.Models;

namespace WineTracker.ViewModels;

public partial class WineViewModel : ObservableValidator
{
    private readonly Wine _wine;

    public WineViewModel() : this(new Wine()) { }

    public WineViewModel(Wine wine)
    {
        _wine = wine;
        _name = wine.Name;
        _description = wine.Description;
        _rating = wine.Rating == 0 ? 1 : wine.Rating;
        _ratingDescription = wine.RatingDescription;
        _winery = wine.Winery;
        _city = wine.City;
        _link = wine.Link;
    }

    public int Id => _wine.Id;
    public DateTime CreatedAt => _wine.CreatedAt;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name cannot be empty.")]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    private int _rating = 1;

    [ObservableProperty]
    private string _ratingDescription = string.Empty;

    [ObservableProperty]
    private string _winery = string.Empty;

    [ObservableProperty]
    private string _city = string.Empty;

    [ObservableProperty]
    private string _link = string.Empty;

    /// <summary>Returns the stars string for display (e.g. "★★★☆☆").</summary>
    public string StarsDisplay
    {
        get
        {
            int r = Math.Clamp(Rating, 1, 5);
            return new string('★', r) + new string('☆', 5 - r);
        }
    }

    /// <summary>Exposes the protected ValidateAllProperties for the view layer.</summary>
    public void TriggerValidation() => ValidateAllProperties();

    /// <summary>Validates link URL if provided.</summary>
    public bool IsLinkValid =>
        string.IsNullOrWhiteSpace(Link) ||
        Uri.TryCreate(Link, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Copies current ViewModel values back to the underlying model
    /// and returns it for persistence.
    /// </summary>
    public Wine ToModel()
    {
        _wine.Name = Name;
        _wine.Description = Description;
        _wine.Rating = Math.Clamp(Rating, 1, 5);
        _wine.RatingDescription = RatingDescription;
        _wine.Winery = Winery;
        _wine.City = City;
        _wine.Link = Link;
        return _wine;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Rating))
            OnPropertyChanged(nameof(StarsDisplay));
    }
}
