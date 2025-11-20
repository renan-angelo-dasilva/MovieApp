using System.Threading.Channels;
using MovieApp.Components.DTOs;

namespace MovieApp.Components.Services;

public interface IMovieService
{
    Task<MovieResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MovieResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MovieResponseDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<MovieResponseDto> CreateAsync(CreateMovieDto createDto, CancellationToken cancellationToken = default);
    Task<MovieResponseDto?> UpdateAsync(int id, UpdateMovieDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    ChannelReader<MovieResponseDto> StreamMoviesByCategoryAsync(string category, CancellationToken cancellationToken = default);
}
