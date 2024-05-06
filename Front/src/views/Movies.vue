<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useMovieStore } from '../store/movieStore';
import { useAlertStore } from '../store/alertStore';
import TableComponent from '../components/common/TableCompontent.vue';
import MovieForm from '../components/forms/MovieForm.vue';
import { Movie } from '../types/models/movie';
import { storeToRefs } from 'pinia';

const { fetchMovies, deleteMovie } = useMovieStore();
const { showConfirm, showToast } = useAlertStore();
const { movies } = storeToRefs(useMovieStore());
const currentMovie = ref<Movie>();
const showForm = ref(false);

onMounted(async () => {
    await fetchMovies();
});

const headers = [
    { title: 'Título', value: 'title' },
    { title: 'Año de Lanzamiento', value: 'releaseYear' },
    { title: 'Director', value: 'directorName' },
    { title: 'Género', value: 'genreName' },
    { title: 'Acciones', align: 'center', value: 'actions', sortable: false }
];

const deleteItem = (item: Movie) => {
    showConfirm("Confirmar eliminación", `¿Estás seguro que deseas eliminar la película ${item.title}?`, async () => {
        try {
            await deleteMovie(item.movieID);
            showToast('Éxito', 'Película eliminada correctamente', 'success');
            await fetchMovies();
        } catch (error: any) {
            showToast('Error', error.message, 'error');
        }
    });
};

const editItem = (item: Movie) => {
    currentMovie.value = item;
    showForm.value = true;
};

const viewerItem = (item: Movie) => {
    var win = window.open("/Viewer/" + item.movieID, '_blank');
    win!.focus();
    // router.push({ name: "Viewer", params: { id: item.movieID } })
};

const resetForm = () => {
    currentMovie.value = {
        movieID: 0,
        posterPath: "",
        releaseYear: 0,
        title: "",
        trailerPath: ""
    };
    showForm.value = true;
};

const onSaved = async () => {
    await fetchMovies();
    showForm.value = false;
};
</script>

<template>
    <MovieForm v-if="showForm" :initialMovie="currentMovie" :onSave="onSaved" />
    <div class="d-flex flex-row justify-center my-8">
        <v-btn v-if="!showForm" size="large" color="blue" @click="resetForm">
            <v-icon>mdi-plus</v-icon>
            Agregar nueva película
        </v-btn>
        <v-btn v-else size="large" color="red" @click="() => showForm = false">
            <v-icon>mdi-close</v-icon>
            Cancelar
        </v-btn>
    </div>
    <TableComponent v-if="!showForm" tableIcon="mdi-film" tableName="Lista de Películas" :headers="headers"
        :items="movies" :editAction="editItem" :deleteAction="deleteItem" :viewAction="viewerItem" />
</template>
