<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useDirectorStore } from '../store/directorStore';
import { useAlertStore } from '../store/alertStore';
import TableComponent from '../components/common/TableCompontent.vue';
import DirectorForm from '../components/forms/DirectorForm.vue';
import { storeToRefs } from 'pinia';

const { fetchDirectors, deleteDirector } = useDirectorStore();
const { showConfirm, showToast } = useAlertStore();
const { directors } = storeToRefs(useDirectorStore());
const currentDirector = ref();
const showForm = ref(false);

onMounted(async () => {
    await fetchDirectors();
});

const headers = [
    { title: 'Nombre', value: 'firstName' },
    { title: 'Apellido', value: 'lastName' },
    { title: 'Fecha de Nacimiento', value: 'birthdate' },
    { title: 'Acciones', align: 'center', value: 'actions', sortable: false }
];

const deleteItem = (item:any) => {
    showConfirm("Confirmar eliminación", `¿Estás seguro que deseas eliminar al director ${item.firstName} ${item.lastName}?`, () => {
        deleteDirector(item.directorID)
            .then(async () => {
                showToast('Éxito', 'Director eliminado correctamente', 'success');
                await fetchDirectors();
            })
            .catch(error => {
                showToast('Error', error.message, 'error');
            });
    });
};

const editItem = (item:any) => {
    currentDirector.value = item;
    showForm.value = true;
};

const resetFields = () => {
    currentDirector.value = { directorID: 0, firstName: '', lastName: '', birthdate: new Date(), biographyPath: "" };
    showForm.value = true;
};

const onSave = async () => {
    await fetchDirectors();
    showForm.value = false;
};
</script>

<template>
    <DirectorForm v-if="showForm" :initialDirector="currentDirector" :onSave="onSave" />
    <div class="d-flex flex-row justify-center my-8">
        <v-btn v-if="!showForm" size="large" color="blue" @click="resetFields">
            <v-icon>mdi-plus</v-icon>
            Agregar nuevo director
        </v-btn>
        <v-btn v-else size="large" color="red" @click="showForm = false">
            <v-icon>mdi-close</v-icon>
            Cancelar
        </v-btn>
    </div>
    <TableComponent v-if="!showForm" tableIcon="mdi-chair-school" tableName="Lista de Directores" :headers="headers"
        :items="directors" :editAction="editItem" :deleteAction="deleteItem" />
</template>
