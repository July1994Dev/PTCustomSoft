<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useGenreStore } from '../store/genreStore';
import { useAlertStore } from '../store/alertStore';
import TableComponent from '../components/common/TableCompontent.vue';
import GenreForm from '../components/forms/GenreForm.vue';
import { storeToRefs } from 'pinia';

const { fetchGenres, deleteGenre } = useGenreStore();
const { showConfirm, showToast } = useAlertStore();
const { genres } = storeToRefs(useGenreStore());
const currentGenre = ref();
const showForm = ref(false);

onMounted(async () => {
    await fetchGenres();
});

const headers = [
    { title: 'Nombre', value: 'name', sortable: true },
    { title: 'Acciones', align: 'center', value: 'actions', sortable: false }
];

const deleteItem = (item: any) => {
    showConfirm("Confirmar eliminación", `¿Estás seguro que deseas eliminar el género ${item.name}?`, () => {
        deleteGenre(item.genreID)
            .then(async () => {
                showToast('Éxito', 'Género eliminado correctamente', 'success');
                await fetchGenres();
            })
            .catch(error => {
                showToast('Error', error.message, 'error');
            });
    });
};

const editItem = (item: any) => {
    currentGenre.value = item;
    showForm.value = true;
};

const resetFields = () => {
    currentGenre.value = { genreID: 0, name: '' };
    showForm.value = true;
};

const onSave = async () => {
    await fetchGenres();
    showForm.value = false;
};
</script>

<template>
    <GenreForm v-if="showForm" :initialGenre="currentGenre" :onSave="onSave" />
    <div class="d-flex flex-row justify-center my-8">
        <v-btn v-if="!showForm" size="large" color="blue" @click="resetFields">
            <v-icon>mdi-plus</v-icon>
            Agregar nuevo género
        </v-btn>
        <v-btn v-else size="large" color="red" @click="showForm = false">
            <v-icon>mdi-close</v-icon>
            Cancelar
        </v-btn>
    </div>
    <TableComponent v-if="!showForm" tableIcon="mdi-tag" tableName="Lista de Géneros" :headers="headers" :items="genres"
        :editAction="editItem" :deleteAction="deleteItem" />
</template>
