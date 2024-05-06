import { defineStore } from 'pinia';
import { ref } from 'vue';
import { userService } from '../services/userService';
import { User } from '../types/models/user';
import { setCookie } from '../utils/cookieUtils';

export const useUserStore = defineStore('user', () => {
    const token = ref<string>('');

    const registerUser = (userData: User):any => {
        return userService.register(userData).then(() => {
            return true;
        }).catch(() => {
            return false;
        });
    };

    const loginUser = async (username: string, password: string): Promise<string | null> => {
        try {
            const response = await userService.login({ username, password });
            token.value = response.data.token;
            setCookie('token', token.value, 1);
            return token.value;
        } catch (error) {
            console.error('Error al iniciar sesión:', error);
            return null;
        }
    };

    const getToken = (): string | null => {
        return token.value;
    };

    return { token, registerUser, loginUser, getToken };
});