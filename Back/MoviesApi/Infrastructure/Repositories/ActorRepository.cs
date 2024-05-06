using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace MoviesApi.Infrastructure.Repositories
{
    public class ActorRepository : IActorRepository
    {
        private readonly string _connectionString;

        public ActorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Actor>> GetAllAsync()
        {
            var actors = new List<Actor>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Actors", connection))
                {
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
                                BiographyPath = reader.GetString(reader.GetOrdinal("BiographyPath"))
                            });
                        }
                    }
                }
            }
            return actors;
        }

        public async Task<Actor> GetByIdAsync(int id)
        {
            Actor actor = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Actors WHERE ActorID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            actor = new Actor
                            {
                                ActorID = reader.GetInt32(reader.GetOrdinal("ActorID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Birthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                                BiographyPath = reader.GetString(reader.GetOrdinal("BiographyPath"))
                            };
                        }
                    }
                }
            }
            return actor;
        }

        public async Task<Actor> CreateAsync(Actor actor)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("INSERT INTO Actors (FirstName, LastName, Birthdate, BiographyPath) VALUES (@FirstName, @LastName, @Birthdate, @BiographyPath) RETURNING ActorID", connection))
                {
                    command.Parameters.AddWithValue("@FirstName", actor.FirstName);
                    command.Parameters.AddWithValue("@LastName", actor.LastName);
                    command.Parameters.AddWithValue("@Birthdate", actor.Birthdate);
                    command.Parameters.AddWithValue("@BiographyPath", actor.BiographyPath);

                    var id = (int)await command.ExecuteScalarAsync();

                    if (id != 0)
                    {
                        actor.ActorID = id;
                        return actor;
                    }
                    else
                    {
                        throw new Exception("No se pudo insertar el actor.");
                    }
                }
            }
        }

        public async Task UpdateAsync(Actor actor)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var isFile = actor.BiographyPath != "" ? ", BiographyPath = @BiographyPath":"";
                using (var command = new NpgsqlCommand($"UPDATE Actors SET FirstName = @FirstName, LastName = @LastName, Birthdate = @Birthdate {isFile} WHERE ActorID = @ActorID", connection))
                {
                    command.Parameters.AddWithValue("@ActorID", actor.ActorID);
                    command.Parameters.AddWithValue("@FirstName", actor.FirstName);
                    command.Parameters.AddWithValue("@LastName", actor.LastName);
                    command.Parameters.AddWithValue("@Birthdate", actor.Birthdate);
                    if (actor.BiographyPath != "") 
                    {
                        command.Parameters.AddWithValue("@BiographyPath", actor.BiographyPath);
                    }
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM Actors WHERE ActorID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId)
        {
            var actors = new List<Actor>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = @"
            SELECT a.ActorID, a.FirstName, a.LastName, a.Birthdate, a.BiographyPath 
            FROM Actors a
            JOIN MovieActors ma ON a.ActorID = ma.ActorID
            WHERE ma.MovieID = @MovieID";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MovieID", movieId);
                    using (var reader = await cmd.ExecuteReaderAsync())
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
    }
}
