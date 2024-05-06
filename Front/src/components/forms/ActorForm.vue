<script setup lang="ts">
import { ref, defineProps, computed } from 'vue';
import { useActorStore } from '../../store/actorStore';
import { useAlertStore } from '../../store/alertStore';
import type { Actor } from '../../types/models/actor';

const props = defineProps({
    initialActor: {
        type: Object as () => Actor,
        default: () => ({
            actorID: 0,
            firstName: '',
            lastName: '',
            birthdate: new Date(), // Establece una fecha por defecto en caso de que no se provea ninguna
            biographyPath: ""
        })
    },
    onSave: Function
});

const valid = ref(false);
const biographyFile = ref<File>();
const actor = ref<Actor>({
    ...props.initialActor,
    birthdate: new Date(props.initialActor.birthdate)
});
const { createActor, updateActor, uploadBiography } = useActorStore();
const { showToast } = useAlertStore();

const formattedBirthdate = computed(() => {
    const date = new Date(actor.value.birthdate);
    if (isNaN(date.getTime())) {
        return actor.value.birthdate.toString().split('T')[0].split('-').reverse().join('/');
    } else {
        return date.toLocaleDateString('es-ES', {
            day: '2-digit', month: '2-digit', year: 'numeric'
        });
    }
});

const submit = () => {
    if (!valid.value) {
        showToast("Campos faltantes", "Por favor completa todos los campos requeridos.", 'warning');
        return;
    }

    const actorToSubmit: Actor = { ...actor.value, birthdate: actor.value.birthdate };

    if (actorToSubmit.actorID) {
        updateActor(actorToSubmit.actorID, actorToSubmit).then(() => {
            showToast("Exito", "Actor actualizado correctamente", 'success');
            if (props.onSave) {
                props.onSave();
            }
        });
    } else {
        createActor(actorToSubmit).then(() => {
            showToast("Exito", "Actor creado correctamente", 'success');
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

        uploadBiography(actor.value.actorID, biographyFile.value)
            .then(() => {
                showToast("Exito", "ABiografía subida correctamente", 'success');

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
        <v-card-title>{{ actor.actorID ? 'Editar Actor' : 'Crear Actor' }}</v-card-title>
        <v-card-text>
            <v-form ref="form" v-model="valid">
                <v-text-field v-model="actor.firstName" label="Nombre" :rules="[v => !!v || 'El nombre es obligatorio']"
                    required></v-text-field>
                <v-text-field v-model="actor.lastName" label="Apellido"
                    :rules="[v => !!v || 'El apellido es obligatorio']" required></v-text-field>
                <v-menu :close-on-content-click="false" close-on-back>
                    <template v-slot:activator="{ props }">
                        <v-text-field v-model="formattedBirthdate" label="Fecha de nacimiento" readonly
                            v-bind="props"></v-text-field>
                    </template>
                    <template #default="{ isActive }">
                        <v-date-picker v-model="actor.birthdate" color="primary"
                            @update:model-value="isActive.value = false">
                        </v-date-picker>
                    </template>
                </v-menu>
                <v-file-input v-if="actor.actorID" v-model="biographyFile" label="Subir biografía"
                    prepend-icon="mdi-file-upload" accept=".pdf" @change="handleUploadBiography"></v-file-input>
            </v-form>
        </v-card-text>
        <v-card-actions>
            <div class="w-100 d-flex justify-center">
                <v-btn size="large" color="primary" @click="submit">Guardar</v-btn>
            </div>
        </v-card-actions>
    </v-card>
</template>
