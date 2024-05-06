<script setup lang="ts">
import { ref, defineProps, computed } from 'vue';
import { useDirectorStore } from '../../store/directorStore';
import { useAlertStore } from '../../store/alertStore';
import type { Director } from '../../types/models/director';

const props = defineProps({
    initialDirector: {
        type: Object as () => Director,
        default: () => ({
            directorID: 0,
            firstName: '',
            lastName: '',
            birthdate: new Date(),
            biographyPath: ""
        })
    },
    onSave: Function
});

const valid = ref(false);
const biographyFile = ref<File>();
const director = ref<Director>({ ...props.initialDirector });
const { createDirector, updateDirector, uploadBiography } = useDirectorStore();
const { showToast } = useAlertStore();

const formattedBirthdate = computed(() => {
    const date = new Date(director.value.birthdate);
    return isNaN(date.getTime()) ?
        director.value.birthdate.toString().split('T')[0].split('-').reverse().join('/') :
        date.toLocaleDateString('es-ES', {
            day: '2-digit', month: '2-digit', year: 'numeric'
        });
});

const submit = () => {
    if (!valid.value) {
        showToast("Campos faltantes", "Por favor completa todos los campos requeridos.", 'warning');
        return;
    }

    const directorToSubmit = { ...director.value, birthdate: director.value.birthdate };
    if (directorToSubmit.directorID) {
        updateDirector(directorToSubmit.directorID, directorToSubmit).then(() => {
            showToast("Éxito", "Director actualizado correctamente", 'success');
            if (props.onSave) {
                props.onSave();
            }
        });
    } else {
        createDirector(directorToSubmit).then(() => {
            showToast("Éxito", "Director creado correctamente", 'success');
            if (props.onSave) {
                props.onSave();
            }
        });
    }
    
};

const handleUploadBiography = () => {
    if (biographyFile.value) {
        const formData = new FormData();
        formData.append('file', biographyFile.value);
        uploadBiography(director.value.directorID, biographyFile.value)
            .then(() => {
                showToast("Éxito", "Biografía subida correctamente", 'success');
            })
            .catch(error => {
                showToast("Error", `Error al subir la biografía: ${error.message}`, 'error');
            });
    } else {
        showToast("Error", `Por favor selecciona un archivo para subir.`, 'warning');
    }
};
</script>

<template>
    <v-card>
        <v-card-title>{{ director.directorID ? 'Editar Director' : 'Crear Director' }}</v-card-title>
        <v-card-text>
            <v-form ref="form" v-model="valid">
                <v-text-field v-model="director.firstName" label="Nombre"
                    :rules="[v => !!v || 'El nombre es obligatorio']" required></v-text-field>
                <v-text-field v-model="director.lastName" label="Apellido"
                    :rules="[v => !!v || 'El apellido es obligatorio']" required></v-text-field>
                <v-menu :close-on-content-click="false" close-on-back>
                    <template v-slot:activator="{ props }">
                        <v-text-field v-model="formattedBirthdate" label="Fecha de nacimiento" readonly
                            v-bind="props"></v-text-field>
                    </template>
                    <template #default="{ isActive }">
                        <v-date-picker v-model="director.birthdate" color="primary"
                            @update:model-value="isActive.value = false">
                        </v-date-picker>
                    </template>
                </v-menu>
                <v-file-input v-if="director.directorID" v-model="biographyFile" label="Subir biografía" prepend-icon="mdi-file-upload"
                    accept=".pdf" @change="handleUploadBiography"></v-file-input>
            </v-form>
        </v-card-text>
        <v-card-actions>
            <div class="w-100 d-flex justify-center">
                <v-btn size="large" color="primary" @click="submit">Guardar</v-btn>
            </div>
        </v-card-actions>
    </v-card>
</template>
