using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories;
using MoviesApi.Infrastructure.Repositories.Interfaces;

namespace MoviesApi.Application.Services
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepository;

        public ActorService(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<IEnumerable<Actor>> GetAllActorsAsync()
        {
            return await _actorRepository.GetAllAsync();
        }

        public async Task<Actor> GetActorByIdAsync(int actorId)
        {
            return await _actorRepository.GetByIdAsync(actorId);
        }

        public async Task<Actor> CreateActorAsync(Actor actor)
        {
            return await _actorRepository.CreateAsync(actor);
        }

        public async Task UpdateActorAsync(Actor actor)
        {
            await _actorRepository.UpdateAsync(actor);
        }

        public async Task DeleteActorAsync(int actorId)
        {
            await _actorRepository.DeleteAsync(actorId);
        }
        public async Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId)
        {
            return await _actorRepository.GetActorsByMovieId(movieId);
        }
    }
}
