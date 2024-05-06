import { Actor } from '../types/models/actor';
import { authApiClient } from '../api/clients';

export const actorService = {
    fetchActors() {
        return authApiClient.get<Actor[]>('/Actors');
    },
    fetchActorById(id: number) {
        return authApiClient.get<Actor>(`/Actors/${id}`);
    },
    createActor(actor: Actor) {
        return authApiClient.post<Actor>('/Actors', actor);
    },
    updateActor(id: number, actor: Actor) {
        return authApiClient.put(`/Actors/${id}`, actor);
    },
    deleteActor(id: number) {
        return authApiClient.delete(`/Actors/${id}`);
    },
    uploadBiography(id: number, formData: FormData) {
        return authApiClient.post(`/Actors/${id}/upload-biography`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    }
};