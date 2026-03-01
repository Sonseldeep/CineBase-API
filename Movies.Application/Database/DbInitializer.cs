using Dapper;

namespace Movies.Application.Database;

public class DbInitializer(IDbConnectionFactory dbConnectionFactory)
{
    public async Task InitializeAsync()
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
    
        await connection.ExecuteAsync($"""
                                       create table if not exists movies (
                                           id UUID primary key,
                                           slug TEXT not null,
                                           title TEXT not null,
                                           yearOfRelease integer not null
                                       ); 
                                       """);
    
        await connection.ExecuteAsync("""
                                      create unique index if not exists movies_slug_idx
                                      on movies
                                      using btree (slug);
                                      """);
        
        
        // note: 
        // in pgsql, it automatically adds on delete cascade when we add a foreign key constraint, so when we delete a movie, all its genres will be deleted as well
        await connection.ExecuteAsync($"""
                                       create table if not exists genres (
                                           movieId UUID references movies (id),
                                           name TEXT not null
                                       ); 
                                       """);
    }

}