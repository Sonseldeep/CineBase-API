using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]

public sealed class MoviesController(IMoviesRepository movieRepository) : ControllerBase
{

    [HttpPost(ApiEndpoints.ApiEndpoints.Movies.Create)]

    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        await movieRepository.CreateAsync(movie);
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [HttpGet(ApiEndpoints.ApiEndpoints.Movies.Get)]

    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = await movieRepository.GetByIdAsync(idOrSlug) 
                    ?? await movieRepository.GetBySlugAsync(idOrSlug);

        if (movie is null)
        {
            return NotFound();
        }
        
        var response = movie.MapToResponse();
        return Ok(response);
    }


    [HttpGet(ApiEndpoints.ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await movieRepository.GetAllAsync();

        var moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);
        var updated = await movieRepository.UpdateAsync(movie);
        if(!updated)
        {
            return NotFound();
        }
        
        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.ApiEndpoints.Movies.Delete)]

    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var deleted = await movieRepository.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}