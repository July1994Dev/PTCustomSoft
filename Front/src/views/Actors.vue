<script setup lang="ts">
import { useActorStore } from '../store/actorStore';
import { useAlertStore } from '../store/alertStore';
import TableComponent from '../components/common/TableCompontent.vue';
import ActorForm from '../components/forms/ActorForm.vue';
import { onMounted, ref } from 'vue';
import { storeToRefs } from 'pinia';

const { fetchActors, deleteActor } = useActorStore();
const { showConfirm, showToast } = useAlertStore();
const { actors } = storeToRefs(useActorStore());
const currentActor = ref();
const showForm = ref(false);

onMounted(async () => {
    await fetchActors();
});

const headers = [
    { title: 'Nombre', value: 'firstName' },
    { title: 'Apellido', value: 'lastName' },
    { title: 'Fecha de Nacimiento', value: 'birthdate' },
    { title: 'Acciones', align: 'center', value: 'actions', sortable: false }
];

const deleteItem = (item: any) => {
    showConfirm("Confirmar eliminación", `¿Estás seguro que deseas eliminar al actor ${item.firstName} ${item.lastName}?`, () => {
        deleteActor(item.actorID)
            .then(async () => {
                showToast('Éxito', 'Actor eliminado correctamente', 'success');
                await fetchActors();
            })
            .catch(error => {
                showToast('Error', error.message, 'error');
            });
    });
};

const editItem = (item: any) => {
    currentActor.value = item;
    showForm.value = true;
};

const resetFields = () => {
    currentActor.value = {
        actorID: 0,
        firstName: '',
        lastName: '',
        birthdate: new Date(),
        biographyPath: ""
    };
    showForm.value = true;
};

const onSave = async () => {
    await fetchActors();
    showForm.value = false;
};
</script>

<template>
    <ActorForm v-if="showForm" :initialActor="currentActor" :onSave="onSave" />
    <div class="d-flex flex-row justify-center my-8">
        <v-btn v-if="!showForm" size="large" color="blue" @click="resetFields">
            <v-icon>mdi-plus</v-icon>
            Agregar nuevo actor
        </v-btn>
        <v-btn v-else size="large" color="red" @click="showForm = false">
            <v-icon>mdi-close</v-icon>
            Cancelar
        </v-btn>
    </div>
    <TableComponent v-if="!showForm" tableIcon="mdi-account" tableName="Lista de Actores" :headers="headers"
        :items="actors" :editAction="editItem" :deleteAction="deleteItem" />
</template>
