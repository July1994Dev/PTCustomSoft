<script setup lang="ts">
import { ref } from 'vue';
import Login from '../components/forms/LoginForm.vue';
import Register from '../components/forms/RegisterForm.vue';

const register = ref(false);

</script>

<template>
    <div class="h-screen d-flex align-center justify-center">
        <div class="d-flex ga-4 flex-column w-100">
            <label class="w-auto text-center text-h6 text-md-h3">
                MOVI<span class="font-weight-bold text-purple-darken-2">APP</span>
            </label>
            <v-card class="mx-auto pa-12 pb-8 w-100 w-md-50" elevation="8" rounded="lg">
                <Login v-if="!register" />
                <Register v-else />
                <v-card-text class="text-center">
                    <a v-if="!register" @click="register = true" class="text-blue text-decoration-none cursor-pointer"
                        rel="noopener noreferrer">
                        Registrarme
                        <v-icon icon="mdi-chevron-right"></v-icon>
                    </a>
                    <a v-else @click="register = false" class="text-blue text-decoration-none cursor-pointer"
                        rel="noopener noreferrer">
                        Iniciar sesion
                        <v-icon icon="mdi-chevron-right"></v-icon>
                    </a>
                </v-card-text>
            </v-card>
        </div>
    </div>
</template>
