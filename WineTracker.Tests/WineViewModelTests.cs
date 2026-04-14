using WineTracker.Models;
using WineTracker.ViewModels;

namespace WineTracker.Tests;

public class WineViewModelTests
{
    [Fact]
    public void Constructor_DefaultValues_AreSet()
    {
        var vm = new WineViewModel();

        Assert.Equal(string.Empty, vm.Name);
        Assert.Equal(1, vm.Rating);
        Assert.Equal(string.Empty, vm.Description);
        Assert.Equal(string.Empty, vm.Winery);
        Assert.Equal(string.Empty, vm.City);
        Assert.Equal(string.Empty, vm.Link);
    }

    [Fact]
    public void Constructor_FromWine_MapsProperties()
    {
        var wine = new Wine
        {
            Id = 42,
            Name = "Bordeaux",
            Description = "A fine red",
            Rating = 4,
            RatingDescription = "Excellent",
            Winery = "Château X",
            City = "Bordeaux",
            Link = "https://example.com",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var vm = new WineViewModel(wine);

        Assert.Equal(42, vm.Id);
        Assert.Equal("Bordeaux", vm.Name);
        Assert.Equal("A fine red", vm.Description);
        Assert.Equal(4, vm.Rating);
        Assert.Equal("Excellent", vm.RatingDescription);
        Assert.Equal("Château X", vm.Winery);
        Assert.Equal("Bordeaux", vm.City);
        Assert.Equal("https://example.com", vm.Link);
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), vm.CreatedAt);
    }

    [Theory]
    [InlineData(0, 1)] // rating of 0 defaults to 1
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(5, 5)]
    public void Constructor_Rating_ClampsZeroToOne(int inputRating, int expectedRating)
    {
        var wine = new Wine { Rating = inputRating };
        var vm = new WineViewModel(wine);

        Assert.Equal(expectedRating, vm.Rating);
    }

    [Theory]
    [InlineData(1, "★☆☆☆☆")]
    [InlineData(2, "★★☆☆☆")]
    [InlineData(3, "★★★☆☆")]
    [InlineData(4, "★★★★☆")]
    [InlineData(5, "★★★★★")]
    public void StarsDisplay_ReturnsCorrectStarString(int rating, string expectedStars)
    {
        var vm = new WineViewModel(new Wine { Rating = rating });

        Assert.Equal(expectedStars, vm.StarsDisplay);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("https://example.com", true)]
    [InlineData("http://example.com", true)]
    [InlineData("not-a-url", false)]
    [InlineData("ftp://example.com", false)]
    public void IsLinkValid_ValidatesUrlCorrectly(string link, bool expectedValid)
    {
        var vm = new WineViewModel { Link = link };

        Assert.Equal(expectedValid, vm.IsLinkValid);
    }

    [Fact]
    public void ToModel_UpdatesUnderlyingWine()
    {
        var wine = new Wine { Name = "Old Name", Rating = 2 };
        var vm = new WineViewModel(wine)
        {
            Name = "New Name",
            Rating = 5,
            Description = "Updated desc",
            Winery = "New Winery",
            City = "New City",
            Link = "https://new.com",
            RatingDescription = "Superb"
        };

        var result = vm.ToModel();

        Assert.Same(wine, result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal(5, result.Rating);
        Assert.Equal("Updated desc", result.Description);
        Assert.Equal("New Winery", result.Winery);
        Assert.Equal("New City", result.City);
        Assert.Equal("https://new.com", result.Link);
        Assert.Equal("Superb", result.RatingDescription);
    }

    [Theory]
    [InlineData(0, 1)]  // clamped to 1
    [InlineData(6, 5)]  // clamped to 5
    [InlineData(3, 3)]  // within range, unchanged
    public void ToModel_ClampsRating(int inputRating, int expectedRating)
    {
        var wine = new Wine();
        var vm = new WineViewModel(wine) { Rating = inputRating };

        var result = vm.ToModel();

        Assert.Equal(expectedRating, result.Rating);
    }

    [Fact]
    public void TriggerValidation_EmptyName_HasErrors()
    {
        var vm = new WineViewModel { Name = string.Empty };

        vm.TriggerValidation();

        Assert.True(vm.HasErrors);
    }

    [Fact]
    public void TriggerValidation_ValidName_HasNoErrors()
    {
        var vm = new WineViewModel { Name = "Valid Wine" };

        vm.TriggerValidation();

        Assert.False(vm.HasErrors);
    }
}
