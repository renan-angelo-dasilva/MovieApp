using Microsoft.EntityFrameworkCore;
using MovieApp.Components.Models;

namespace MovieApp.Components.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Category);
            entity.Property(e => e.Cast).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
        });

        // Seed data
        modelBuilder.Entity<Movie>().HasData(
            new Movie
            {
                Id = 1,
                Title = "The Shawshank Redemption",
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                Category = "Drama",
                ReleaseYear = 1994,
                Rating = 9.3,
                MinimumAge = 13,
                Director = "Frank Darabont",
                Cast = new() { "Tim Robbins", "Morgan Freeman" },
                DurationMinutes = 142
            },
            new Movie
            {
                Id = 2,
                Title = "The Dark Knight",
                Description = "When the menace known as the Joker wreaks havoc on Gotham, Batman must accept one of the greatest tests of his ability to fight injustice.",
                Category = "Action",
                ReleaseYear = 2008,
                Rating = 9.0,
                MinimumAge = 13,
                Director = "Christopher Nolan",
                Cast = new() { "Christian Bale", "Heath Ledger" },
                DurationMinutes = 152
            },
            new Movie
            {
                Id = 3,
                Title = "Toy Story",
                Description = "A cowboy doll is profoundly threatened when a new spaceman figure supplants him as top toy in a boy's room.",
                Category = "Animation",
                ReleaseYear = 1995,
                Rating = 8.3,
                MinimumAge = 0,
                Director = "John Lasseter",
                Cast = new() { "Tom Hanks", "Tim Allen" },
                DurationMinutes = 81
            }
        );
    }
}
