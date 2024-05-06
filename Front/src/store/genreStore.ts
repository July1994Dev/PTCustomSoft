import { defineStore } from 'pinia';
import { ref } from 'vue';
import { genreService } from '../services/genreService';
import { Genre } from '../types/models/genre';

export const useGenreStore = defineStore('genre', () => {
    const genres = ref<Genre[]>([]);

    const fetchGenres = async (): Promise<Genre[]> => {
        try {
            const response = await genreService.fetchGenres();
            genres.value = response.data || [];
            return genres.value;
        } catch (error) {
            console.error('Error al obtener géneros:', error);
            return [];
        }
    };

    const fetchGenreById = async (id: number): Promise<Genre | null> => {
        try {
            const response = await genreService.fetchGenreById(id);
            return response.data || null;
        } catch (error) {
            console.error('Error al obtener el género:', error);
            return null;
        }
    };

    const createGenre = async (genre: Genre): Promise<Genre | null> => {
        try {
            const response = await genreService.createGenre(genre);
            return response.data;
        } catch (error) {
            console.error('Error al crear género:', error);
            return null;
        }
    };

    const updateGenre = async (id: number, genre: Genre): Promise<Genre | null> => {
        try {
            const response = await genreService.updateGenre(id, genre);
            return response.data;
        } catch (error) {
            console.error('Error al actualizar género:', error);
            return null;
        }
    };

    const deleteGenre = async (id: number): Promise<boolean> => {
        try {
            await genreService.deleteGenre(id);
            return true;
        } catch (error) {
            console.error('Error al eliminar género:', error);
            return false;
        }
    };

    return { genres, fetchGenres, fetchGenreById, createGenre, updateGenre, deleteGenre };
});
