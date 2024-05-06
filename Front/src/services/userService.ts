import { User } from '../types/models/user';
import { apiClient } from '../api/clients';

export const userService = {
    register(user: User) {
        return apiClient.post('/api/User/register', user);
    },
    login(credentials: { username: string, password: string }) {
        return apiClient.post('/api/User/login', credentials);
    }
};
