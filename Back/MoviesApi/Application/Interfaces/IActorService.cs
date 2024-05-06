using MoviesApi.Domain.Models;

namespace MoviesApi.Application.Interfaces
{
    public interface IActorService
    {
        Task<IEnumerable<Actor>> GetAllActorsAsync();
        Task<Actor> GetActorByIdAsync(int actorId);
        Task<Actor> CreateActorAsync(Actor actor);
        Task UpdateActorAsync(Actor actor);
        Task DeleteActorAsync(int actorId);
        Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId);
    }
}
