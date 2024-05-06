using MoviesApi.Domain.Models;

namespace MoviesApi.Application.Interfaces
{
    public interface IDirectorService
    {
        Task<IEnumerable<Director>> GetAllDirectorsAsync();
        Task<Director> GetDirectorByIdAsync(int directorId);
        Task<Director> CreateDirectorAsync(Director director);
        Task UpdateDirectorAsync(Director director);
        Task DeleteDirectorAsync(int directorId);
    }
}
