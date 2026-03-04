namespace Movies.Contracts.Responses;

public class MovieResponse
{
    public required Guid Id { get; init; }
    
    public required string Title { get; init; }
    public required string Slug { get; init; }

    // general rating of the movie, calculated as an average of all user ratings
    public float? Rating { get; init; }
    // rating given by the user who made the request, if any
    public int? UserRating { get; init; }
    public required int YearOfRelease { get; init; }
    public required IEnumerable<string> Genres { get; init; } = [];
}