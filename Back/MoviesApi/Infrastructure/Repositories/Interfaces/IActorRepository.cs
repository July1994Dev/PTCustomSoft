using MoviesApi.Domain.Models;

namespace MoviesApi.Infrastructure.Repositories.Interfaces
{
    public interface IActorRepository
    {
        Task<IEnumerable<Actor>> GetAllAsync();
        Task<Actor> GetByIdAsync(int id);
        Task<Actor> CreateAsync(Actor actor);
        Task UpdateAsync(Actor actor);
        Task DeleteAsync(int id);
        Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId);
    }
}
