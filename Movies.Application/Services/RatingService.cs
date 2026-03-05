using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMoviesRepository _moviesRepository;

    public RatingService(IRatingRepository ratingRepository, IMoviesRepository moviesRepository)
    {
        _ratingRepository = ratingRepository;
        _moviesRepository = moviesRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException([
                new ValidationFailure
                {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            ]);  
        }


    
        var movieExists = await _moviesRepository.ExistsByIdAsync(movieId, token);
        if (!movieExists)
        {
            return false;
        }
        return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);
    }
}