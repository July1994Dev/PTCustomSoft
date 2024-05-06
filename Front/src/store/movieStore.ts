import { defineStore } from 'pinia';
import { ref } from 'vue';
import { movieService } from '../services/movieService';
import { Movie } from '../types/models/movie';
import { Actor } from '../types/models/actor';

export const useMovieStore = defineStore('movie', () => {
    const movies = ref<Movie[]>([]);
    const currentMovieActors = ref<Actor[]>([]);

    const fetchMovies = async (): Promise<Movie[]> => {
        try {
            const response = await movieService.fetchMovies();
            movies.value = response.data || [];
            return movies.value;
        } catch (error) {
            console.error('Error al obtener películas:', error);
            return [];
        }
    };

    const fetchMovieById = async (id: number): Promise<Movie | null> => {
        try {
            const response = await movieService.fetchMovieById(id);
            return response.data || null;
        } catch (error) {
            console.error('Error al obtener la película:', error);
            return null;
        }
    };

    const createMovie = async (movie: Movie): Promise<Movie | null> => {
        try {
            const response = await movieService.createMovie(movie);
            return response.data;
        } catch (error) {
            console.error('Error al crear película:', error);
            return null;
        }
    };

    const updateMovie = async (id: number, movie: Movie): Promise<Movie | null> => {
        try {
            const response = await movieService.updateMovie(id, movie);
            return response.data;
        } catch (error) {
            console.error('Error al actualizar película:', error);
            return null;
        }
    };

    const deleteMovie = async (id: number): Promise<boolean> => {
        try {
            await movieService.deleteMovie(id);
            return true;
        } catch (error) {
            console.error('Error al eliminar película:', error);
            return false;
        }
    };

    const uploadPoster = async (id: number, file: File): Promise<string | null> => {
        const formData = new FormData();
        formData.append('file', file);
        const response = await movieService.uploadPoster(id, formData);
        return response.data.path || null;
    };

    const uploadTrailer = async (id: number, file: File): Promise<string | null> => {
        const formData = new FormData();
        formData.append('file', file);
        const response = await movieService.uploadTrailer(id, formData);
        return response.data.path || null;
    };

    const fetchActorsForMovie = async (movieId: number) => {
        try{
            currentMovieActors.value = await movieService.getActorsByMovieId(movieId);
        }catch{
            currentMovieActors.value = [];
        }
    };

    const addActorToMovie = async (movieId: number, actorId: number[]) => {
        await movieService.updateMovieActors(movieId, actorId);
        await fetchActorsForMovie(movieId);
    }

    return { movies, currentMovieActors, fetchMovies, fetchMovieById, createMovie, updateMovie, deleteMovie, uploadPoster, uploadTrailer, addActorToMovie, fetchActorsForMovie };
});
