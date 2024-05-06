using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;

namespace MoviesApi.Application.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }

        public async Task<Genre> GetGenreByIdAsync(int genreId)
        {
            return await _genreRepository.GetByIdAsync(genreId);
        }

        public async Task<Genre> CreateGenreAsync(Genre genre)
        {
            return await _genreRepository.CreateAsync(genre);
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            await _genreRepository.UpdateAsync(genre);
        }

        public async Task DeleteGenreAsync(int genreId)
        {
            await _genreRepository.DeleteAsync(genreId);
        }
    }
}
