using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMoviesRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        // Create a newconnection
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        // begin a transaction
        using var transaction = connection.BeginTransaction();
        // Insert the movie
        // we can skip commandDefination, but it allows to pass cancellation token
        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            insert into movies (id, slug, title, yearofrelease)
            values(@Id, @Slug, @Title, @YearOfRelease)
            """, movie, cancellationToken: token
            ));

        // Insert the genres
        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    """
                    insert into genres (movieId, name)
                    values(@MovieId, @Name)
                    """, new { MovieId = movie.Id, Name = genre },
                    cancellationToken: token
                    ));
            }
        }
        // commit the transaction
        transaction.Commit();
        return result > 0;

    }

    public async Task<Movie?> GetByIdAsync(Guid id,Guid? userid = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select m.* ,
                                         round(avg(r.rating), 1) as rating,
                                         myr.rating as userrating
                                  from movies m
                                  left join ratings r on m.id = r.movieId
                                  left join ratings myr on m.id = myr.movieId
                                      and myr.userid = @userId
                                  where id = @id
                                  group by id, userrating
                                  """, new {id, userid},
                                  cancellationToken: token
                                  ));

        if (movie is null)
        {
            return null;
        }
        
        // if the movies exist get the genres
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                                   
                                  select name from genres
                                  where movieid = @id
                                  """, new {id},
                                  cancellationToken: token
                                  ));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }
        return movie;
        
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select m.* ,
                                         round(avg(r.rating), 1) as rating,
                                         myr.rating as userrating
                                  from movies m
                                  left join ratings r on m.id = r.movieId
                                  left join ratings myr on m.id = myr.movieId
                                      and myr.userid = @userId
                                  where slug = @slug
                                  group by id, userrating
                                  """, new {slug, userId},
                                  cancellationToken: token
                                  ));

        if (movie is null)
        {
            return null;
        }
        
        // if the movies exist get the genres
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                                   
                                  select name from genres
                                  where movieid = @id
                                  """, new {id = movie.Id},
                                  cancellationToken: token
                                  ));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }
        return movie;


    }

    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.QueryAsync(new CommandDefinition("""
                   select m.*,
                          string_agg(distinct g.name, ',') as genres,
                          round(avg(r.rating), 1) as rating,
                          myr.rating as userrating
                   from movies m 
                   left join genres g on m.id = g.movieid
                   left join ratings r on m.id = r.movieid
                   left join ratings myr on m.id = myr.movieid
                   and myr.userid = @userId
                   group by id,userrating
                   """,new {userId}, cancellationToken: token));
    
        return result.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            YearOfRelease = m.yearofrelease,
            Rating = (float?)m.rating,
            UserRating = (int?)m.userrating,
            Genres = Enumerable.ToList(m.genres.Split(','))
        });
        
    }

    public async  Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
              delete from genres 
              where movieid = @id
              """, new {id = movie.Id},
                cancellationToken: token
        ));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                  insert into genres (movieId, name)
                  values (@MovieId, @Name)
                  """, new { MovieId = movie.Id, Name = genre },
                    cancellationToken: token
                ));
                                                               
        }
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies
            set 
                slug = @Slug,
                title = @Title,
                yearOfRelease = @YearOfRelease
            
            where id = @Id
            """, movie,
            cancellationToken: token
        ));

        transaction.Commit();
        return result > 0;
        
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        
        // first delete all the genres of the movie because of the foreign key constraint
        await connection.ExecuteAsync(new CommandDefinition("""
              delete from genres 
              where movieid = @id
              """, new {id},
                cancellationToken: token
        ));
        
        var result =  await connection.ExecuteAsync(new CommandDefinition("""
              delete from movies 
              where id = @id
              """, new {id},
                cancellationToken: token
        ));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1)
            from movies
            where id = @id
            """, new {id},
            cancellationToken: token
        ));
        return result;
    }
}