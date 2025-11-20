namespace MovieApp.Components.Models;

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public int ReleaseYear { get; set; }
    public double Rating { get; set; }
    public int MinimumAge { get; set; }
    public string? Director { get; set; }
    public List<string> Cast { get; set; } = new();
    public int DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
