namespace MoviesApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string username, string email, string password);
        Task<(bool Success, string Token)> AuthenticateUserAsync(string username, string password);
    }
}
