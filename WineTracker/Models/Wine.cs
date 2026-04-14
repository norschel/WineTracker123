using System;
using System.ComponentModel.DataAnnotations;

namespace WineTracker.Models;

public class Wine
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>Rating from 1 to 5.</summary>
    public int Rating { get; set; } = 1;

    public string RatingDescription { get; set; } = string.Empty;

    public string Winery { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
