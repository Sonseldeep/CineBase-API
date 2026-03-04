using FluentValidation;
using Movies.Application.Database;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesRepository _moviesRepository;
    // inject the validator to validate the movie before creating or updating it
    private readonly IValidator<Movie> _movieValidator;
    private readonly IRatingRepository _ratingRepository;

    public MovieService(IMoviesRepository moviesRepository, IValidator<Movie> movieValidator, IRatingRepository ratingRepository)
    {
        _moviesRepository = moviesRepository;
        _movieValidator = movieValidator;
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
       
        movie.Genres = movie.Genres
            .Where(g => !string.IsNullOrWhiteSpace(g.Trim()))
            .Select(g => g.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();
        
        // validate the movie before creating it
        // and catch in mapping in API layer
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
        return  await _moviesRepository.CreateAsync(movie, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id,Guid? userid = default, CancellationToken token = default)
    {
        return  _moviesRepository.GetByIdAsync(id,userid, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token = default)
    {
        return  _moviesRepository.GetBySlugAsync(slug,userid, token);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(Guid? userid = default, CancellationToken token = default)
    {
        return  _moviesRepository.GetAllAsync(userid,token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie,Guid? userid = default, CancellationToken token = default)
    {
        // validate the movie before updating it
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
        var movieExists = await _moviesRepository.ExistsByIdAsync(movie.Id, token);
        if (!movieExists)
        {
            return null;
        }

        await _moviesRepository.UpdateAsync(movie, token);

        if (!userid.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }
        var ratings = await _ratingRepository.GetRatingAsync(movie.Id,userid.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return  _moviesRepository.DeleteByIdAsync(id, token);
    }
}