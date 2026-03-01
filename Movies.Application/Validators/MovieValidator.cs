using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

// Note: movievalidator is used without
// calling it directly
public class MovieValidator : AbstractValidator<Movie>
{

    private readonly IMoviesRepository _movieRepository;
    
    public MovieValidator(IMoviesRepository moviesRepository)
    {
        _movieRepository = moviesRepository;
        RuleFor(m => m.Id)
            .NotEmpty();

        RuleForEach(m => m.Genres)
            .NotEmpty();
          

        RuleFor(m => m.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(m => m.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(m => m.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system");


    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token=default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug);
        if (existingMovie is not null)
        {
            return existingMovie.Id == movie.Id;
        }

        return existingMovie is null;
    }
}
