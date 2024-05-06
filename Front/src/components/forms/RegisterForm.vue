<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useUserStore } from '../../store/userStore';
import { useAlertStore } from '../../store/alertStore';
import type { User } from '../../types/models/user';
import router from '../../router';

const visible = ref(false);
const user = reactive<User>({
    email: "",
    password: "",
    username: ""
});

const { registerUser } = useUserStore();
const { showAlert, showConfirm } = useAlertStore();

const handleLogin = async () => {
    if (!user.email || !user.password || !user.username) {
        showAlert("Datos incompletos", "Es necesario ingresar todos los campos", "warning");
        return;
    }

    const result = await registerUser(user);

    if (!result) {
        showAlert("Registro fallido", "Ya existe un usuario con estos datos.", "error");
    } else {
        showConfirm("Correcto", "Los datos fueron registrados correctamente, porfavor inicia sesion.", ()=>{
            router.go(0);
        });
    }
};
</script>

<template>
    <div class="text-subtitle-1 text-medium-emphasis">Usuario</div>
    <v-text-field v-model="user.username" type="text" density="compact" placeholder="Nombre de usuario"
        prepend-inner-icon="mdi-account-outline" variant="outlined"></v-text-field>

    <div class="text-subtitle-1 text-medium-emphasis">Correo</div>
    <v-text-field v-model="user.email" type="email" density="compact" placeholder="Correo electronico"
        prepend-inner-icon="mdi-email-outline" variant="outlined"></v-text-field>

    <div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
        Contraseña
    </div>
    <v-text-field v-model="user.password" :append-inner-icon="visible ? 'mdi-eye-off' : 'mdi-eye'"
        :type="visible ? 'text' : 'password'" density="compact" placeholder="Ingresa tu contraseña"
        prepend-inner-icon="mdi-lock-outline" variant="outlined"
        @click:append-inner="visible = !visible"></v-text-field>
    <v-btn @click="handleLogin" class="mb-8" color="blue" size="large" variant="tonal" block>
        Registrarme
    </v-btn>
</template>