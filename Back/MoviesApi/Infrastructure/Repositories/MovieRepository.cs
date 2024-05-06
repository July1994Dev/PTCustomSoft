using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;
using Npgsql;
using System.IO;

namespace MoviesApi.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly string _connectionString;

        public MovieRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            var movies = new List<Movie>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Movies", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            movies.Add(new Movie
                            {
                                MovieID = reader.GetInt32(reader.GetOrdinal("MovieID")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                ReleaseYear = reader.GetInt32(reader.GetOrdinal("ReleaseYear")),
                                DirectorID = reader.GetInt32(reader.GetOrdinal("DirectorID")),
                                GenreID = reader.GetInt32(reader.GetOrdinal("GenreID")),
                                PosterPath = reader.GetString(reader.GetOrdinal("PosterPath")),
                                TrailerPath = reader.GetString(reader.GetOrdinal("TrailerPath"))
                            });
                        }
                    }
                }
            }
            return movies;
        }

        public async Task<Movie> GetByIdAsync(int id)
        {
            Movie movie = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Movies WHERE MovieID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            movie = new Movie
                            {
                                MovieID = reader.GetInt32(reader.GetOrdinal("MovieID")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                ReleaseYear = reader.GetInt32(reader.GetOrdinal("ReleaseYear")),
                                DirectorID = reader.GetInt32(reader.GetOrdinal("DirectorID")),
                                GenreID = reader.GetInt32(reader.GetOrdinal("GenreID")),
                                PosterPath = reader.GetString(reader.GetOrdinal("PosterPath")),
                                TrailerPath = reader.GetString(reader.GetOrdinal("TrailerPath"))
                            };
                        }
                    }
                }
            }
            return movie;
        }

        public async Task<Movie> CreateAsync(Movie movie)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("INSERT INTO Movies (Title, ReleaseYear, DirectorID, GenreID, PosterPath, TrailerPath) VALUES (@Title, @ReleaseYear, @DirectorID, @GenreID, @PosterPath, @TrailerPath) RETURNING MovieID", connection))
                {
                    command.Parameters.AddWithValue("@Title", movie.Title);
                    command.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
                    command.Parameters.AddWithValue("@DirectorID", movie.DirectorID);
                    command.Parameters.AddWithValue("@GenreID", movie.GenreID);
                    command.Parameters.AddWithValue("@PosterPath", movie.PosterPath);
                    command.Parameters.AddWithValue("@TrailerPath", movie.TrailerPath);
                    var id = (int)await command.ExecuteScalarAsync();

                    // Retrieve the newly created movie to return it
                    if (id != 0)
                    {
                        movie.MovieID = id;
                        return movie;
                    }
                    else
                    {
                        throw new Exception("No se pudo insertar la pelicula.");
                    }
                }
            }
        }

        public async Task UpdateAsync(Movie movie)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var isTrailer = movie.TrailerPath != "" ? ", TrailerPath = @TrailerPath" : "";
                var isPoster = movie.PosterPath != "" ? ", PosterPath = @PosterPath" : "";

                using (var command = new NpgsqlCommand($"UPDATE Movies SET Title = @Title, ReleaseYear = @ReleaseYear, DirectorID = @DirectorID, GenreID = @GenreID {isPoster} {isTrailer} WHERE MovieID = @MovieID", connection))
                {
                    command.Parameters.AddWithValue("@MovieID", movie.MovieID);
                    command.Parameters.AddWithValue("@Title", movie.Title);
                    command.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
                    command.Parameters.AddWithValue("@DirectorID", movie.DirectorID);
                    command.Parameters.AddWithValue("@GenreID", movie.GenreID);
                    command.Parameters.AddWithValue("@PosterPath", movie.PosterPath);
                    command.Parameters.AddWithValue("@TrailerPath", movie.TrailerPath);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM Movies WHERE MovieID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<Movie>> GetMoviesByDirectorId(int directorId)
        {
            var movies = new List<Movie>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = @"SELECT m.MovieID, m.Title, m.GenreID, m.DirectorID FROM Movies m WHERE m.DirectorID = @DirectorID";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DirectorID", directorId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            movies.Add(new Movie
                            {
                                MovieID = reader.GetInt32(reader.GetOrdinal("MovieID")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                GenreID = reader.GetInt32(reader.GetOrdinal("GenreID")),
                                DirectorID = reader.GetInt32(reader.GetOrdinal("DirectorID")),
                            });
                        }
                    }
                }
            }
            return movies;
        }
        public async Task AddActorToMovie(int movieId, int actorId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("INSERT INTO movieactors (movieid, actorid) VALUES (@movieid, @actorid)", connection))
                {
                    command.Parameters.AddWithValue("@movieid", movieId);
                    command.Parameters.AddWithValue("@actorid", actorId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId)
        {
            var actors = new List<Actor>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"SELECT a.* FROM Actors a INNER JOIN movieactors ma ON a.ActorID = ma.actorid WHERE ma.movieid = @MovieID";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MovieID", movieId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            actors.Add(new Actor
                            {
                                ActorID = reader.GetInt32(reader.GetOrdinal("ActorID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Birthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                                BiographyPath = reader.IsDBNull(reader.GetOrdinal("BiographyPath")) ? null : reader.GetString(reader.GetOrdinal("BiographyPath"))
                            });
                        }
                    }
                }
            }
            return actors;
        }
        public async Task UpdateMovieActorsAsync(int movieId, List<int> actorIds)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    // Eliminar actores existentes
                    using (var deleteCommand = new NpgsqlCommand("DELETE FROM MovieActors WHERE MovieId = @MovieId", connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@MovieId", movieId);
                        await deleteCommand.ExecuteNonQueryAsync();
                    }

                    // Insertar nuevos actores
                    foreach (var actorId in actorIds)
                    {
                        using (var insertCommand = new NpgsqlCommand("INSERT INTO MovieActors (MovieId, ActorId) VALUES (@MovieId, @ActorId)", connection))
                        {
                            insertCommand.Parameters.AddWithValue("@MovieId", movieId);
                            insertCommand.Parameters.AddWithValue("@ActorId", actorId);
                            await insertCommand.ExecuteNonQueryAsync();
                        }
                    }

                    await transaction.CommitAsync();
                }
            }
        }
    }
}
