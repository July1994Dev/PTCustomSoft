using MoviesApi.Domain.Models;

namespace MoviesApi.Infrastructure.Repositories.Interfaces
{
    public interface IDirectorRepository
    {
        Task<IEnumerable<Director>> GetAllAsync();
        Task<Director> GetByIdAsync(int id);
        Task<Director> CreateAsync(Director director);
        Task UpdateAsync(Director director);
        Task DeleteAsync(int id);
    }
}
