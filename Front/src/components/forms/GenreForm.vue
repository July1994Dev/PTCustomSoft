<script setup lang="ts">
import { ref, defineProps } from 'vue';
import { useGenreStore } from '../../store/genreStore';
import { useAlertStore } from '../../store/alertStore';
import type { Genre } from '../../types/models/genre';

const props = defineProps({
    initialGenre: {
        type: Object as () => Genre,
        default: () => ({
            genreID: 0,
            name: ''
        })
    },
    onSave: Function
});

const valid = ref(false);
const genre = ref<Genre>({ ...props.initialGenre });
const { createGenre, updateGenre } = useGenreStore();
const { showToast } = useAlertStore();

const submit = () => {
    if (!valid.value) {
        showToast("Campos faltantes", "Por favor completa todos los campos requeridos.", 'warning');
        return;
    }

    const genreToSubmit = { ...genre.value };
    if (genreToSubmit.genreID) {
        updateGenre(genreToSubmit.genreID, genreToSubmit).then(() => {
            showToast("Éxito", "Género actualizado correctamente", 'success');
            if (props.onSave) {
                props.onSave();
            }
        });
    } else {
        createGenre(genreToSubmit).then(() => {
            showToast("Éxito", "Género creado correctamente", 'success');
            if (props.onSave) {
                props.onSave();
            }
        });
    }
    
};
</script>

<template>
    <v-card>
        <v-card-title>{{ genre.genreID ? 'Editar Género' : 'Crear Género' }}</v-card-title>
        <v-card-text>
            <v-form ref="form" v-model="valid">
                <v-text-field v-model="genre.name" label="Nombre" :rules="[v => !!v || 'El nombre es obligatorio']"
                    required></v-text-field>
            </v-form>
        </v-card-text>
        <v-card-actions>
            <div class="w-100 d-flex justify-center">
                <v-btn size="large" color="primary" @click="submit">Guardar</v-btn>
            </div>
        </v-card-actions>
    </v-card>
</template>
