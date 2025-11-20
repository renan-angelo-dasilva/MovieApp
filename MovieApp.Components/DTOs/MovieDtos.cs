namespace MovieApp.Components.DTOs;

public record CreateMovieDto(
    string Title,
    string Description,
    string Category,
    int ReleaseYear,
    double Rating,
    int MinimumAge,
    string? Director,
    List<string>? Cast,
    int DurationMinutes
);

public record UpdateMovieDto(
    string? Title,
    string? Description,
    string? Category,
    int? ReleaseYear,
    double? Rating,
    int? MinimumAge,
    string? Director,
    List<string>? Cast,
    int? DurationMinutes
);

public record MovieResponseDto(
    int Id,
    string Title,
    string Description,
    string Category,
    int ReleaseYear,
    double Rating,
    int MinimumAge,
    string? Director,
    List<string> Cast,
    int DurationMinutes,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MovieRecommendationRequest(int UserAge);

public record MovieRecommendationResponse(
    List<MovieResponseDto> RecommendedMovies,
    string Reasoning
);
