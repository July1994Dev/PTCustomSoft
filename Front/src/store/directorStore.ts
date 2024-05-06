import { defineStore } from 'pinia';
import { ref } from 'vue';
import { directorService } from '../services/directorService';
import { Director } from '../types/models/director';

export const useDirectorStore = defineStore('director', () => {
    const directors = ref<Director[]>([]);

    const fetchDirectors = async (): Promise<Director[]> => {
        try {
            const response = await directorService.fetchDirectors();
            directors.value = response.data || [];
            return directors.value;
        } catch (error) {
            console.error('Error al obtener directores:', error);
            return [];
        }
    };

    const fetchDirectorById = async (id: number): Promise<Director | null> => {
        try {
            const response = await directorService.fetchDirectorById(id);
            return response.data || null;
        } catch (error) {
            console.error('Error al obtener el director:', error);
            return null;
        }
    };

    const createDirector = async (director: Director): Promise<Director | null> => {
        try {
            const response = await directorService.createDirector(director);
            return response.data;
        } catch (error) {
            console.error('Error al crear director:', error);
            return null;
        }
    };

    const updateDirector = async (id: number, director: Director): Promise<Director | null> => {
        try {
            const response = await directorService.updateDirector(id, director);
            return response.data;
        } catch (error) {
            console.error('Error al actualizar director:', error);
            return null;
        }
    };

    const deleteDirector = async (id: number): Promise<boolean> => {
        try {
            await directorService.deleteDirector(id);
            return true;
        } catch (error) {
            console.error('Error al eliminar director:', error);
            return false;
        }
    };

    const uploadBiography = async (id: number, file: File): Promise<string | null> => {
        const formData = new FormData();
        formData.append('file', file);
        const response = await directorService.uploadBiography(id, formData);
        return response.data.path || null;
    };
    return { directors, fetchDirectors, fetchDirectorById, createDirector, updateDirector, deleteDirector, uploadBiography };
});
