using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using MovieApp.Components.Data;
using MovieApp.Components.DTOs;
using MovieApp.Components.Models;

namespace MovieApp.Components.Services;

public class MovieService : IMovieService
{
    private readonly MovieDbContext _context;

    public MovieService(MovieDbContext context) => _context = context;

    public async Task<MovieResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var movie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);
        return movie != null ? ToDto(movie) : null;
    }

    public async Task<IEnumerable<MovieResponseDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _context.Movies.AsNoTracking().OrderByDescending(m => m.Rating).ToListAsync(ct))
        .Select(ToDto);

    public async Task<IEnumerable<MovieResponseDto>> GetByCategoryAsync(string category, CancellationToken ct = default) =>
        (await _context.Movies.AsNoTracking()
            .Where(m => m.Category.ToLower() == category.ToLower())
            .OrderByDescending(m => m.Rating)
            .ToListAsync(ct))
        .Select(ToDto);

    public async Task<MovieResponseDto> CreateAsync(CreateMovieDto dto, CancellationToken ct = default)
    {
        var movie = new Movie
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            ReleaseYear = dto.ReleaseYear,
            Rating = dto.Rating,
            MinimumAge = dto.MinimumAge,
            Director = dto.Director,
            Cast = dto.Cast ?? new(),
            DurationMinutes = dto.DurationMinutes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync(ct);
        return ToDto(movie);
    }

    public async Task<MovieResponseDto?> UpdateAsync(int id, UpdateMovieDto dto, CancellationToken ct = default)
    {
        var movie = await _context.Movies.FindAsync(new object[] { id }, ct);
        if (movie == null) return null;

        if (!string.IsNullOrEmpty(dto.Title)) movie.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description)) movie.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.Category)) movie.Category = dto.Category;
        if (dto.ReleaseYear.HasValue) movie.ReleaseYear = dto.ReleaseYear.Value;
        if (dto.Rating.HasValue) movie.Rating = dto.Rating.Value;
        if (dto.MinimumAge.HasValue) movie.MinimumAge = dto.MinimumAge.Value;
        if (dto.Director != null) movie.Director = dto.Director;
        if (dto.Cast != null) movie.Cast = dto.Cast;
        if (dto.DurationMinutes.HasValue) movie.DurationMinutes = dto.DurationMinutes.Value;
        
        movie.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
        return ToDto(movie);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var movie = await _context.Movies.FindAsync(new object[] { id }, ct);
        if (movie == null) return false;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public ChannelReader<MovieResponseDto> StreamMoviesByCategoryAsync(string category, CancellationToken ct = default)
    {
        var channel = Channel.CreateUnbounded<MovieResponseDto>();

        _ = Task.Run(async () =>
        {
            try
            {
                await foreach (var movie in _context.Movies.AsNoTracking()
                    .Where(m => m.Category.ToLower() == category.ToLower())
                    .OrderByDescending(m => m.Rating)
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    await channel.Writer.WriteAsync(ToDto(movie), ct);
                    await Task.Delay(100, ct); // Simulate streaming
                }
            }
            finally
            {
                channel.Writer.Complete();
            }
        }, ct);

        return channel.Reader;
    }

    private static MovieResponseDto ToDto(Movie m) => new(
        m.Id, m.Title, m.Description, m.Category, m.ReleaseYear, m.Rating,
        m.MinimumAge, m.Director, m.Cast, m.DurationMinutes, m.CreatedAt, m.UpdatedAt
    );
}
