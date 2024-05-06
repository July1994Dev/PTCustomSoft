using Npgsql;
using BCrypt.Net;
using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("SELECT * FROM Users WHERE Username = @Username", connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                        };
                    }
                }
            }
        }
        return null;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("SELECT * FROM Users WHERE Email = @Email", connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                        };
                    }
                }
            }
        }
        return null;
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)", connection))
            {
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }
}
