import { Director } from '../types/models/director';
import { authApiClient } from '../api/clients';

export const directorService = {
    fetchDirectors() {
        return authApiClient.get<Director[]>('/Directors');
    },
    fetchDirectorById(id: number) {
        return authApiClient.get<Director>(`/Directors/${id}`);
    },
    createDirector(director: Director) {
        return authApiClient.post<Director>('/Directors', director);
    },
    updateDirector(id: number, director: Director) {
        return authApiClient.put(`/Directors/${id}`, director);
    },
    deleteDirector(id: number) {
        return authApiClient.delete(`/Directors/${id}`);
    },
    uploadBiography(id: number, formData: FormData) {
        return authApiClient.post(`/Directors/${id}/upload-biography`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    },
    fetchMoviesByDirectorId(directorId: number) {
        return authApiClient.get(`/Directors/${directorId}/Movies`);
    }
};