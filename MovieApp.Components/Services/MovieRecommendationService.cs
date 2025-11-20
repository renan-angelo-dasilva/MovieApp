using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MovieApp.Components.DTOs;
using System.Text;

namespace MovieApp.Components.Services;

#pragma warning disable SKEXP0110 // Agent APIs are experimental

public class MovieRecommendationService : IMovieRecommendationService
{
    private readonly IMovieService _movieService;
    private readonly IKernelFactory _kernelFactory;
    private readonly ILogger<MovieRecommendationService> _logger;

    public MovieRecommendationService(IMovieService movieService, IKernelFactory kernelFactory, ILogger<MovieRecommendationService> logger)
    {
        _movieService = movieService;
        _kernelFactory = kernelFactory;
        _logger = logger;
    }

    public async Task<MovieRecommendationResponse> GetRecommendationsAsync(int userAge, CancellationToken ct = default)
    {
        try
        {
            var allMovies = (await _movieService.GetAllAsync(ct)).ToList();
            var ageAppropriate = allMovies.Where(m => m.MinimumAge <= userAge).ToList();
            
            if (!ageAppropriate.Any())
                return new MovieRecommendationResponse(new List<MovieResponseDto>(), "No age-appropriate movies found.");

            var kernel = _kernelFactory.CreateKernel();

            // Create three specialized agents
            var romanceAgent = new ChatCompletionAgent
            {
                Name = "RomanceExpert",
                Instructions = "You are a romance movie specialist. Analyze the catalog and recommend ONE romance movie suitable for the user's age. " +
                              "Focus on romantic themes, love stories, or strong romantic elements. Respond with ONLY the movie ID number.",
                Kernel = kernel,
                Arguments = new KernelArguments(
                    new OpenAIPromptExecutionSettings()
                    {
                        MaxTokens = 4096,
                        Temperature = 0.4f
                    })
            };

            var actionAgent = new ChatCompletionAgent
            {
                Name = "ActionHeroExpert",
                Instructions = "You are an action and superhero movie specialist. Analyze the catalog and recommend ONE action or superhero movie. " +
                              "Look for movies with Action category or superhero themes. Respond with ONLY the movie ID number.",
                Kernel = kernel
            };

            var classicAgent = new ChatCompletionAgent
            {
                Name = "ClassicCinemaExpert",
                Instructions = "You are a classic cinema specialist. Analyze the catalog and recommend ONE classic movie (released before 2010) with high ratings. " +
                              "Focus on timeless films. Respond with ONLY the movie ID number.",
                Kernel = kernel
            };

            // Prepare catalog for agents
            var catalogText = BuildCatalog(ageAppropriate, userAge);

            var recommendations = await GetConcurrentRecommendationsAsync(
                romanceAgent, actionAgent, classicAgent, 
                catalogText, ageAppropriate, ct);

            if (!recommendations.Any())
            {
                // Fallback to top rated
                recommendations = ageAppropriate
                    .OrderByDescending(m => m.Rating)
                    .Take(3)
                    .ToList();
                return new MovieRecommendationResponse(recommendations, "Fallback: Top rated movies for your age.");
            }

            var reasoning = BuildReasoningFromMovies(recommendations);
            return new MovieRecommendationResponse(recommendations, reasoning);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Multi-agent orchestration failed");
            var fallback = (await _movieService.GetAllAsync(ct))
                .Where(m => m.MinimumAge <= userAge)
                .OrderByDescending(m => m.Rating)
                .Take(3)
                .ToList();
            return new MovieRecommendationResponse(fallback, "Fallback: Top rated movies.");
        }
    }

    private static string BuildCatalog(List<MovieResponseDto> movies, int userAge)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"User Age: {userAge}");
        sb.AppendLine("Available Movies:");
        foreach (var movie in movies)
        {
            sb.AppendLine($"ID:{movie.Id} | {movie.Title} | Category:{movie.Category} | Year:{movie.ReleaseYear} | Rating:{movie.Rating}/10 | MinAge:{movie.MinimumAge}+");
        }
        return sb.ToString();
    }

    private async Task<List<MovieResponseDto>> GetConcurrentRecommendationsAsync(
        ChatCompletionAgent romanceAgent,
        ChatCompletionAgent actionAgent,
        ChatCompletionAgent classicAgent,
        string catalog,
        List<MovieResponseDto> availableMovies,
        CancellationToken ct)
    {
        var agentHistory = new List<ChatMessageContent>();
        var recommendations = new List<MovieResponseDto>();

        try
        {
          
            var concurrentOrchestration = new ConcurrentOrchestration(classicAgent, romanceAgent, actionAgent)
            {
                ResponseCallback = (response) =>
                {
                    agentHistory.Add(response);
                    return ValueTask.CompletedTask;
                }
            };

            var runtime = new InProcessRuntime();
            await runtime.StartAsync();

            var input = $"Suggest a movie to watch based on the catalog:\n{catalog}";
            await concurrentOrchestration.InvokeAsync(input, runtime);
            await runtime.RunUntilIdleAsync();

           
            foreach (var message in agentHistory)
            {
                if (message.Content != null)
                {
                    var movieId = ExtractMovieId(message.Content);
                    if (movieId.HasValue)
                    {
                        var movie = availableMovies.FirstOrDefault(m => m.Id == movieId.Value);
                        if (movie != null && !recommendations.Any(r => r.Id == movie.Id))
                        {
                            recommendations.Add(movie);
                        }
                    }
                }
            }

            if (recommendations.Count < 3)
            {
                ApplyFallbackRecommendations(availableMovies, recommendations);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Concurrent orchestration failed, using fallback");
            ApplyFallbackRecommendations(availableMovies, recommendations);
        }

        return recommendations;
    }

    private static void ApplyFallbackRecommendations(List<MovieResponseDto> availableMovies, List<MovieResponseDto> recommendations)
    {
        // Romance fallback
        var romanceMovie = availableMovies.FirstOrDefault(m =>
            m.Category.Contains("Romance", StringComparison.OrdinalIgnoreCase));
        if (romanceMovie != null && !recommendations.Any(r => r.Id == romanceMovie.Id))
            recommendations.Add(romanceMovie);

        // Action fallback
        var actionMovie = availableMovies.FirstOrDefault(m =>
            m.Category.Contains("Action", StringComparison.OrdinalIgnoreCase) ||
            m.Title.Contains("Knight", StringComparison.OrdinalIgnoreCase));
        if (actionMovie != null && !recommendations.Any(r => r.Id == actionMovie.Id))
            recommendations.Add(actionMovie);

        // Classic fallback
        var classicMovie = availableMovies
            .Where(m => m.ReleaseYear < 2010)
            .OrderByDescending(m => m.Rating)
            .FirstOrDefault();
        if (classicMovie != null && !recommendations.Any(r => r.Id == classicMovie.Id))
            recommendations.Add(classicMovie);
    }

    private static int? ExtractMovieId(string response)
    {
        // Try to find any number in the response
        var numbers = System.Text.RegularExpressions.Regex.Matches(response, @"\b\d+\b");
        if (numbers.Count > 0 && int.TryParse(numbers[0].Value, out int id))
        {
            return id;
        }
        return null;
    }

    private static string BuildReasoningFromMovies(List<MovieResponseDto> recommendations)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Multi-agent concurrent recommendations:");

        foreach (var movie in recommendations)
        {
            var icon = movie.Category switch
            {
                var c when c.Contains("Romance", StringComparison.OrdinalIgnoreCase) => "🌹",
                var c when c.Contains("Action", StringComparison.OrdinalIgnoreCase) => "💥",
                _ => "🎬"
            };

            sb.AppendLine($"{icon} {movie.Title} ({movie.Category}) - Rating: {movie.Rating}/10 - Year: {movie.ReleaseYear}");
        }

        return sb.ToString();
    }
}

#pragma warning restore SKEXP0110
