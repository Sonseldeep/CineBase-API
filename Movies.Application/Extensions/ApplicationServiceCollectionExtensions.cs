using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        
        services.AddSingleton<IMoviesRepository, MovieRepository>();
        services.AddSingleton<IMovieService,MovieService>();
    }

    public static void AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
    }
    
}