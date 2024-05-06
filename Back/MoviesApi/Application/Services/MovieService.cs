using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;

namespace MoviesApi.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _movieRepository.GetAllAsync();
        }

        public async Task<Movie> GetMovieByIdAsync(int movieId)
        {
            return await _movieRepository.GetByIdAsync(movieId);
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            return await _movieRepository.CreateAsync(movie);
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            await _movieRepository.UpdateAsync(movie);
        }

        public async Task DeleteMovieAsync(int movieId)
        {
            await _movieRepository.DeleteAsync(movieId);
        }
        public async Task<IEnumerable<Movie>> GetMoviesByDirectorId(int directorId)
        {
            return await _movieRepository.GetMoviesByDirectorId(directorId);
        }
        public async Task AddActorToMovie(int movieId, int actorId)
        {
            await _movieRepository.AddActorToMovie(movieId, actorId);
        }
        public async Task<IEnumerable<Actor>> GetActorsByMovieId(int movieId)
        {
            return await _movieRepository.GetActorsByMovieId(movieId);
        }
        public async Task UpdateMovieActorsAsync(int movieId, List<int> actorIds)
        {
            await _movieRepository.UpdateMovieActorsAsync(movieId, actorIds);
        }
    }
}
