using MoviesApi.Domain.Models;

namespace MoviesApi.Infrastructure.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie> GetByIdAsync(int id);
        Task<Movie> CreateAsync(Movie movie);
        Task UpdateAsync(Movie movie);
        Task DeleteAsync(int id);
        Task<IEnumerable<Movie>> GetMoviesByDirectorId(int directorId);
        Task AddActorToMovie(int movieId, int actorId);
        Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId);
        Task UpdateMovieActorsAsync(int movieId, List<int> actorIds);
    }
}
