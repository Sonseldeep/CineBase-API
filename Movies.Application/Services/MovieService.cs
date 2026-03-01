using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesRepository _moviesRepository;
    // inject the validator to validate the movie before creating or updating it
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMoviesRepository moviesRepository, IValidator<Movie> movieValidator)
    {
        _moviesRepository = moviesRepository;
        _movieValidator = movieValidator;
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
       
        movie.Genres = movie.Genres
            .Where(g => !string.IsNullOrWhiteSpace(g.Trim()))
            .Select(g => g.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();
        
        // validate the movie before creating it
        // and catch in mapping in API layer
        await _movieValidator.ValidateAndThrowAsync(movie);
        return  await _moviesRepository.CreateAsync(movie);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return  _moviesRepository.GetByIdAsync(id);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return  _moviesRepository.GetBySlugAsync(slug);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return  _moviesRepository.GetAllAsync();
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        // validate the movie before updating it
        await _movieValidator.ValidateAndThrowAsync(movie);
        var movieExists = await _moviesRepository.ExistsByIdAsync(movie.Id);
        if (!movieExists)
        {
            return null;
        }

        await _moviesRepository.UpdateAsync(movie);
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return  _moviesRepository.DeleteByIdAsync(id);
    }
}