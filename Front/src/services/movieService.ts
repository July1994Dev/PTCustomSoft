import { Movie } from '../types/models/movie';
import { authApiClient } from '../api/clients';
import { Actor } from '../types/models/actor';

export const movieService = {
    fetchMovies() {
        return authApiClient.get<Movie[]>('/Movies');
    },
    fetchMovieById(id: number) {
        return authApiClient.get<Movie>(`/Movies/${id}`);
    },
    createMovie(movie: Movie) {
        return authApiClient.post<Movie>('/Movies', movie);
    },
    updateMovie(id: number, movie: Movie) {
        return authApiClient.put(`/Movies/${id}`, movie);
    },
    deleteMovie(id: number) {
        return authApiClient.delete(`/Movies/${id}`);
    },
    uploadPoster(id: number, formData: FormData) {
        return authApiClient.post(`/Movies/${id}/upload-poster`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    },
    uploadTrailer(id: number, formData: FormData) {
        return authApiClient.post(`/Movies/${id}/upload-trailer`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    },
    fetchActorsByMovieId(movieId: number) {
        return authApiClient.get(`/Movies/${movieId}/Actors`);
    },
    addActorToMovie(movieId: number, actorId: number): Promise<void> {
        return authApiClient.post(`/movies/${movieId}/actors/${actorId}`).then(response => response.data);
    },
    getActorsByMovieId(movieId: number): Promise<Actor[]> {
        return authApiClient.get(`/movies/${movieId}/actors`).then(response => response.data);
    },
    updateMovieActors(movieId: number, actorIds: number[]) {
        return authApiClient.post(`/movies/${movieId}/actors`, actorIds);
    }
};
