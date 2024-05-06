using MoviesApi.Application.Interfaces;
using MoviesApi.Domain.Models;
using MoviesApi.Infrastructure.Repositories.Interfaces;

namespace MoviesApi.Application.Services
{
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorRepository _directorRepository;

        public DirectorService(IDirectorRepository directorRepository)
        {
            _directorRepository = directorRepository;
        }

        public async Task<IEnumerable<Director>> GetAllDirectorsAsync()
        {
            return await _directorRepository.GetAllAsync();
        }

        public async Task<Director> GetDirectorByIdAsync(int directorId)
        {
            return await _directorRepository.GetByIdAsync(directorId);
        }

        public async Task<Director> CreateDirectorAsync(Director director)
        {
            return await _directorRepository.CreateAsync(director);
        }

        public async Task UpdateDirectorAsync(Director director)
        {
            await _directorRepository.UpdateAsync(director);
        }

        public async Task DeleteDirectorAsync(int directorId)
        {
            await _directorRepository.DeleteAsync(directorId);
        }
    }
}
