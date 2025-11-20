using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Components.DTOs;
using MovieApp.Components.Services;

namespace MovieApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IMovieRecommendationService _recommendationService;
    private readonly IValidator<CreateMovieDto> _createValidator;
    private readonly IValidator<UpdateMovieDto> _updateValidator;

    public MoviesController(
        IMovieService movieService,
        IMovieRecommendationService recommendationService,
        IValidator<CreateMovieDto> createValidator,
        IValidator<UpdateMovieDto> updateValidator)
    {
        _movieService = movieService;
        _recommendationService = recommendationService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Get all movies
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all movies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MovieResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieResponseDto>>> GetAllMovies(CancellationToken ct)
    {
        var movies = await _movieService.GetAllAsync(ct);
        return Ok(movies);
    }

    /// <summary>
    /// Get a movie by ID
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Movie details</returns>
    [HttpGet("{id:int}", Name = nameof(GetMovieById))]
    [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieResponseDto>> GetMovieById(int id, CancellationToken ct)
    {
        var movie = await _movieService.GetByIdAsync(id, ct);
        
        if (movie is null)
        {
            return NotFound(new { message = "Movie not found" });
        }

        return Ok(movie);
    }

    /// <summary>
    /// Get all movies in a specific category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of movies in the category</returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<MovieResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieResponseDto>>> GetMoviesByCategory(
        string category, 
        CancellationToken ct)
    {
        var movies = await _movieService.GetByCategoryAsync(category, ct);
        return Ok(movies);
    }

    /// <summary>
    /// Stream movies by category using Server-Sent Events
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="ct">Cancellation token</param>
    [HttpGet("category/{category}/stream")]
    [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status200OK)]
    public async Task StreamMoviesByCategory(string category, CancellationToken ct)
    {
        Response.ContentType = "text/event-stream";
        var channelReader = _movieService.StreamMoviesByCategoryAsync(category, ct);

        await foreach (var movie in channelReader.ReadAllAsync(ct))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(movie);
            await Response.WriteAsync($"data: {json}\n\n", ct);
            await Response.Body.FlushAsync(ct);
        }
    }

    /// <summary>
    /// Add a new movie to the catalog
    /// </summary>
    /// <param name="createDto">Movie creation data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created movie</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieResponseDto>> CreateMovie(
        [FromBody] CreateMovieDto createDto,
        CancellationToken ct)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto, ct);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return ValidationProblem(ModelState);
        }

        var movie = await _movieService.CreateAsync(createDto, ct);
        return CreatedAtRoute(nameof(GetMovieById), new { id = movie.Id }, movie);
    }

    /// <summary>
    /// Update an existing movie
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="updateDto">Movie update data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated movie</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieResponseDto>> UpdateMovie(
        int id,
        [FromBody] UpdateMovieDto updateDto,
        CancellationToken ct)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto, ct);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return ValidationProblem(ModelState);
        }

        var movie = await _movieService.UpdateAsync(id, updateDto, ct);
        
        if (movie is null)
        {
            return NotFound(new { message = "Movie not found" });
        }

        return Ok(movie);
    }

    /// <summary>
    /// Delete a movie from the catalog
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovie(int id, CancellationToken ct)
    {
        var deleted = await _movieService.DeleteAsync(id, ct);
        
        if (!deleted)
        {
            return NotFound(new { message = "Movie not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Get AI-powered movie recommendations based on user age
    /// </summary>
    /// <param name="request">Recommendation request with user age</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Movie recommendations with reasoning</returns>
    [HttpPost("recommendations")]
    [ProducesResponseType(typeof(MovieRecommendationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieRecommendationResponse>> GetMovieRecommendations(
        [FromBody] MovieRecommendationRequest request,
        CancellationToken ct)
    {
        if (request.UserAge < 0 || request.UserAge > 120)
        {
            return BadRequest(new { message = "User age must be between 0 and 120" });
        }

        var recommendations = await _recommendationService.GetRecommendationsAsync(request.UserAge, ct);
        return Ok(recommendations);
    }
}
