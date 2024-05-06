using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace MoviesApi.Infrastructure.Repositories
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly string _connectionString;

        public DirectorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Director>> GetAllAsync()
        {
            var directors = new List<Director>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Directors", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            directors.Add(new Director
                            {
                                DirectorID = reader.GetInt32(reader.GetOrdinal("DirectorID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Birthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                                BiographyPath = reader.GetString(reader.GetOrdinal("BiographyPath"))
                            });
                        }
                    }
                }
            }
            return directors;
        }

        public async Task<Director> GetByIdAsync(int id)
        {
            Director director = null;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM Directors WHERE DirectorID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            director = new Director
                            {
                                DirectorID = reader.GetInt32(reader.GetOrdinal("DirectorID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Birthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                                BiographyPath = reader.GetString(reader.GetOrdinal("BiographyPath"))
                            };
                        }
                    }
                }
            }
            return director;
        }

        public async Task<Director> CreateAsync(Director director)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("INSERT INTO Directors (FirstName, LastName, Birthdate, BiographyPath) VALUES (@FirstName, @LastName, @Birthdate, @BiographyPath) RETURNING DirectorID", connection))
                {
                    command.Parameters.AddWithValue("@FirstName", director.FirstName);
                    command.Parameters.AddWithValue("@LastName", director.LastName);
                    command.Parameters.AddWithValue("@Birthdate", director.Birthdate);
                    command.Parameters.AddWithValue("@BiographyPath", director.BiographyPath);

                    var id = (int)await command.ExecuteScalarAsync();

                    if (id != 0)
                    {
                        director.DirectorID = id;
                        return director;
                    }
                    else
                    {
                        throw new Exception("No se pudo insertar el director.");
                    }
                }
            }
        }

        public async Task UpdateAsync(Director director)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var isFile = director.BiographyPath != "" ? ", BiographyPath = @BiographyPath" : "";
                using (var command = new NpgsqlCommand($"UPDATE Directors SET FirstName = @FirstName, LastName = @LastName, Birthdate = @Birthdate {isFile} WHERE DirectorID = @DirectorID", connection))
                {
                    command.Parameters.AddWithValue("@DirectorID", director.DirectorID);
                    command.Parameters.AddWithValue("@FirstName", director.FirstName);
                    command.Parameters.AddWithValue("@LastName", director.LastName);
                    command.Parameters.AddWithValue("@Birthdate", director.Birthdate);
                    command.Parameters.AddWithValue("@BiographyPath", director.BiographyPath);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM Directors WHERE DirectorID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
