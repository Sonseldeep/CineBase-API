using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.AssemblyMarker;
using Movies.Application.Database;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IRatingRepository, RatingRepository>();
        services.AddSingleton<IMoviesRepository, MovieRepository>();
        services.AddSingleton<IMovieService,MovieService>();
        // register all validators in the assembly as singleton
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
    }

    public static void AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
    }
    
}