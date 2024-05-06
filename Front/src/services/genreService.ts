import { Genre } from '../types/models/genre';
import { authApiClient } from '../api/clients';

export const genreService = {
    fetchGenres() {
        return authApiClient.get<Genre[]>('/Genres');
    },
    fetchGenreById(id: number) {
        return authApiClient.get<Genre>(`/Genres/${id}`);
    },
    createGenre(genre: Genre) {
        return authApiClient.post<Genre>('/Genres', genre);
    },
    updateGenre(id: number, genre: Genre) {
        return authApiClient.put(`/Genres/${id}`, genre);
    },
    deleteGenre(id: number) {
        return authApiClient.delete(`/Genres/${id}`);
    }
};
