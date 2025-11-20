using FluentValidation;
using MovieApp.Components.DTOs;

namespace MovieApp.Components.Validators;

public class CreateMovieDtoValidator : AbstractValidator<CreateMovieDto>
{
    public CreateMovieDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.ReleaseYear).InclusiveBetween(1888, DateTime.UtcNow.Year + 2);
        RuleFor(x => x.Rating).InclusiveBetween(0.0, 10.0);
        RuleFor(x => x.MinimumAge).InclusiveBetween(0, 21);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
    }
}

public class UpdateMovieDtoValidator : AbstractValidator<UpdateMovieDto>
{
    public UpdateMovieDtoValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Title));
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description));
        RuleFor(x => x.ReleaseYear).InclusiveBetween(1888, DateTime.UtcNow.Year + 2).When(x => x.ReleaseYear.HasValue);
        RuleFor(x => x.Rating).InclusiveBetween(0.0, 10.0).When(x => x.Rating.HasValue);
        RuleFor(x => x.MinimumAge).InclusiveBetween(0, 21).When(x => x.MinimumAge.HasValue);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).When(x => x.DurationMinutes.HasValue);
    }
}
