// src/store/actorStore.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { actorService } from '../services/actorService';
import { Actor } from '../types/models/actor';

export const useActorStore = defineStore('actor', () => {
    const actors = ref<Actor[]>([]);

    const fetchActors = async (): Promise<Actor[]> => {
        try {
            const response = await actorService.fetchActors();
            actors.value = response.data || [];
            return actors.value;
        } catch (error) {
            console.error('Error al obtener actores:', error);
            return [];
        }
    };

    const fetchActorById = async (id: number): Promise<Actor | null> => {
        try {
            const response = await actorService.fetchActorById(id);
            return response.data || null;
        } catch (error) {
            console.error('Error al obtener el actor:', error);
            return null;
        }
    };

    const createActor = async (actor: Actor): Promise<Actor | null> => {
        try {
            const response = await actorService.createActor(actor);
            return response.data;
        } catch (error) {
            console.error('Error al crear actor:', error);
            return null;
        }
    };

    const updateActor = async (id: number, actor: Actor): Promise<Actor | null> => {
        try {
            const response = await actorService.updateActor(id, actor);
            return response.data;
        } catch (error) {
            console.error('Error al actualizar actor:', error);
            return null;
        }
    };

    const deleteActor = async (id: number): Promise<boolean> => {
        try {
            await actorService.deleteActor(id);
            return true;
        } catch (error) {
            console.error('Error al eliminar actor:', error);
            return false;
        }
    };

    const uploadBiography = async (id: number, file: File): Promise<string | null> => {
        const formData = new FormData();
        formData.append('file', file);
        const response = await actorService.uploadBiography(id, formData);
        return response.data.path || null;
    };

    return { actors, fetchActors, fetchActorById, createActor, updateActor, deleteActor, uploadBiography };
});
