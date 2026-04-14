using WineTracker.Models;

namespace WineTracker.Tests;

public class WineModelTests
{
    [Fact]
    public void Wine_DefaultValues_AreSet()
    {
        var wine = new Wine();

        Assert.Equal(0, wine.Id);
        Assert.Equal(string.Empty, wine.Name);
        Assert.Equal(string.Empty, wine.Description);
        Assert.Equal(1, wine.Rating);
        Assert.Equal(string.Empty, wine.RatingDescription);
        Assert.Equal(string.Empty, wine.Winery);
        Assert.Equal(string.Empty, wine.City);
        Assert.Equal(string.Empty, wine.Link);
    }

    [Fact]
    public void Wine_CanSetAllProperties()
    {
        var now = DateTime.UtcNow;
        var wine = new Wine
        {
            Id = 1,
            Name = "Pinot Noir",
            Description = "Light and fruity",
            Rating = 3,
            RatingDescription = "Good",
            Winery = "Some Winery",
            City = "Napa",
            Link = "https://example.com/wine",
            CreatedAt = now
        };

        Assert.Equal(1, wine.Id);
        Assert.Equal("Pinot Noir", wine.Name);
        Assert.Equal("Light and fruity", wine.Description);
        Assert.Equal(3, wine.Rating);
        Assert.Equal("Good", wine.RatingDescription);
        Assert.Equal("Some Winery", wine.Winery);
        Assert.Equal("Napa", wine.City);
        Assert.Equal("https://example.com/wine", wine.Link);
        Assert.Equal(now, wine.CreatedAt);
    }
}
