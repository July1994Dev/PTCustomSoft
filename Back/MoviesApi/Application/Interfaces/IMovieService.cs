using MoviesApi.Domain.Models;

namespace MoviesApi.Application.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<Movie> GetMovieByIdAsync(int movieId);
        Task<Movie> CreateMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(int movieId);
        Task<IEnumerable<Movie>> GetMoviesByDirectorId(int directorId);
        Task AddActorToMovie(int movieId, int actorId);
        Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId);
        Task UpdateMovieActorsAsync(int movieId, List<int> actorIds);
    }
}
