using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMoviesRepository
{
    Task<bool> CreateAsync(Movie movie);
    Task<Movie?> GetByIdAsync(string id);
    Task<Movie?> GetBySlugAsync(string slug);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(string id);

}