<script setup lang="ts">
import { ref } from 'vue';
import { useUserStore } from '../../store/userStore';
import { useAlertStore } from '../../store/alertStore';

const email = ref('');
const password = ref('');
const visible = ref(false);
const userStore = useUserStore();
const { showAlert } = useAlertStore();

const handleLogin = async () => {
    if (!email.value || !password.value) {
        showAlert("Datos incompletos", "Es necesario ingresar todos los campos", "warning");
        return;
    }

    const result = await userStore.loginUser(email.value, password.value);

    if (!result) {
        showAlert("Datos incorrectos", "Las credenciales que ingresaste no son validas, verifica tus datos", "error");
    } else {
        window.location.href = "/Dashboard/Directors";
    }
};
</script>

<template>
    <div class="text-subtitle-1 text-medium-emphasis">Correo</div>
    <v-text-field v-model="email" type="email" density="compact" placeholder="Correo electronico"
        prepend-inner-icon="mdi-email-outline" variant="outlined"></v-text-field>

    <div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
        Contraseña
    </div>
    <v-text-field v-model="password" :append-inner-icon="visible ? 'mdi-eye-off' : 'mdi-eye'"
        :type="visible ? 'text' : 'password'" density="compact" placeholder="Ingresa tu contraseña"
        prepend-inner-icon="mdi-lock-outline" variant="outlined"
        @click:append-inner="visible = !visible"></v-text-field>
    <v-btn @click="handleLogin" class="mb-8" color="blue" size="large" variant="tonal" block>
        Entrar
    </v-btn>
</template>