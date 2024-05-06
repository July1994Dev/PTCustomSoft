using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace MoviesApi.Infrastructure.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly string _connectionString;

        public GenreRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            var genres = new List<Genre>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Genres", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            genres.Add(new Genre
                            {
                                GenreID = reader.GetInt32(reader.GetOrdinal("GenreID")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                    }
                }
            }
            return genres;
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            Genre genre = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Genres WHERE GenreID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            genre = new Genre
                            {
                                GenreID = reader.GetInt32(reader.GetOrdinal("GenreID")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                    }
                }
            }
            return genre;
        }

        public async Task<Genre> CreateAsync(Genre genre)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("INSERT INTO Genres (Name) VALUES (@Name) RETURNING GenreID", connection))
                {
                    command.Parameters.AddWithValue("@Name", genre.Name);

                    var id = (int)await command.ExecuteScalarAsync();

                    if (id != 0)
                    {
                        genre.GenreID = id;
                        return genre;
                    }
                    else
                    {
                        throw new Exception("No se pudo insertar el género.");
                    }
                }
            }
        }
        public async Task UpdateAsync(Genre genre)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("UPDATE Genres SET Name = @Name WHERE GenreID = @GenreID", connection))
                {
                    command.Parameters.AddWithValue("@GenreID", genre.GenreID);
                    command.Parameters.AddWithValue("@Name", genre.Name);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM Genres WHERE GenreID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
