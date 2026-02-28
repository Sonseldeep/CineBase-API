using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        services.AddSingleton<IMoviesRepository, MovieRepository>();
        return services;
    }
}