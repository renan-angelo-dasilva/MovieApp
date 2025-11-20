using MovieApp.Components.DTOs;

namespace MovieApp.Components.Services;

public interface IMovieRecommendationService
{
    Task<MovieRecommendationResponse> GetRecommendationsAsync(int userAge, CancellationToken cancellationToken = default);
}
