using MoviesApi.Domain.Models;

namespace MoviesApi.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
    }
}
