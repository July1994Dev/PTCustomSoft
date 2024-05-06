<script setup lang="ts">
import { ref, defineProps, onMounted } from 'vue';
import { useMovieStore } from '../../store/movieStore';
import { useAlertStore } from '../../store/alertStore';
import { useDirectorStore } from '../../store/directorStore';
import { useGenreStore } from '../../store/genreStore';
import { useActorStore } from '../../store/actorStore';
import { storeToRefs } from 'pinia';
import { Movie } from '../../types/models/movie';

const props = defineProps({
    initialMovie: {
        type: Object as () => Movie,
        default: () => ({
            movieID: 0,
            title: '',
            releaseYear: new Date().getFullYear(),
            directorID: 0,
            genreID: 0,
            posterPath: '',
            trailerPath: ''
        })
    },
    onSave: Function
});

const valid = ref(false);
const posterFile = ref<File>();
const trailerFile = ref<File>();
const movie = ref<Movie>({ ...props.initialMovie });
const { currentMovieActors } = storeToRefs(useMovieStore());
const { createMovie, updateMovie, uploadPoster, uploadTrailer, addActorToMovie, fetchActorsForMovie } = useMovieStore();
const { showToast } = useAlertStore();
const { directors } = storeToRefs(useDirectorStore());
const { genres } = storeToRefs(useGenreStore());
const { fetchActors } = useActorStore();
const { actors } = storeToRefs(useActorStore());
const { fetchDirectors } = useDirectorStore();
const { fetchGenres } = useGenreStore();
const selectedActorIds = ref<number[]>([]);

const fetchRelatedData = async () => {
    await Promise.all([
        fetchDirectors(),
        fetchGenres(),
        fetchActors()
    ]);
};

onMounted(async()=>{
    await fetchRelatedData();
    if(movie.value.movieID){
       await fetchActorsForMovie(movie.value.movieID);
        selectedActorIds.value = currentMovieActors.value.map(actor => actor.actorID);
    }
});

const submit = async () => {
    if (!valid.value) {
        showToast("Campos faltantes", "Por favor completa todos los campos requeridos.", 'warning');
        return;
    }

    const movieToSubmit = { ...movie.value };

    try {
        if (movieToSubmit.movieID) {
            await updateMovie(movie.value.movieID, movieToSubmit);
            showToast("Éxito", "Película actualizada correctamente", 'success');
            await addActorToMovie(movie.value.movieID, selectedActorIds.value);
        } else {
            const newMovie = await createMovie(movieToSubmit);
            movie.value.movieID = newMovie!.movieID;
            showToast("Éxito", "Película creada correctamente", 'success');
        }

        if (posterFile.value && movie.value.movieID) {
            await uploadPoster(movie.value.movieID, posterFile.value);
            showToast("Éxito", "Póster subido correctamente", 'success');
        }
        if (trailerFile.value && movie.value.movieID) {
            await uploadTrailer(movie.value.movieID, trailerFile.value);
            showToast("Éxito", "Tráiler subido correctamente", 'success');
        }

        if (props.onSave) {
            props.onSave();
        }
    } catch (error: any) {
        showToast("Error", `Error al procesar la película: ${error.message}`, 'error');
    }
};
</script>

<template>
    <v-card>
        <v-card-title>{{ movie.movieID ? 'Editar Película' : 'Crear Película' }}</v-card-title>
        <v-card-text>
            <v-form ref="form" v-model="valid">
                <v-text-field v-model="movie.title" label="Título" :rules="[v => !!v || 'El título es obligatorio']"
                    required></v-text-field>
                <v-text-field type="number" v-model="movie.releaseYear" label="Año de Lanzamiento"
                    :rules="[v => !!v || 'El año es obligatorio']" required></v-text-field>
                <v-select label="Director" v-model="movie.directorID" :items="directors"
                    :item-title="item => `${item.firstName} ${item.lastName}`" item-value="directorID"
                    :rules="[v => !!v || 'El director es obligatorio']"></v-select>
                <v-select label="Género" v-model="movie.genreID" :items="genres" item-title="name" item-value="genreID"
                    :rules="[v => !!v || 'El género es obligatorio']"></v-select>
                <v-select label="Actores" v-model="selectedActorIds" :items="actors"
                    :item-title="item => `${item.firstName} ${item.lastName}`" item-value="actorID" multiple></v-select>
                <v-file-input v-if="movie.movieID" v-model="posterFile" label="Subir Póster" prepend-icon="mdi-image"
                    accept="image/*"></v-file-input>
                <v-file-input v-if="movie.movieID" v-model="trailerFile" label="Subir Tráiler" prepend-icon="mdi-video"
                    accept="video/*"></v-file-input>
            </v-form>
        </v-card-text>
        <v-card-actions>
            <div class="w-100 d-flex justify-center">
                <v-btn size="large" color="primary" @click="submit">Guardar</v-btn>
            </div>
        </v-card-actions>
    </v-card>
</template>
