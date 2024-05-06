<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { onMounted } from 'vue';
import { useGenreStore } from '../store/genreStore';
import { useMovieStore } from '../store/movieStore';
import { useDirectorStore } from '../store/directorStore';
import { Director } from '../types/models/director';
import { Movie } from '../types/models/movie';
import { Genre } from '../types/models/genre';
import Background from '../assets/img/background.jpg';

const route = useRoute();
const movieId = route.params.id;
const showActors = ref(false);
const ActiveMovie = ref<Movie>();
const ActiveDirector = ref<Director>();
const ActiveGenre = ref<Genre>();
let serverURL = import.meta.env.VITE_API_URL;
const { currentMovieActors } = storeToRefs(useMovieStore());
const { fetchGenreById } = useGenreStore();
const { fetchMovieById, fetchActorsForMovie } = useMovieStore();
const { fetchDirectorById } = useDirectorStore();

onMounted(async () => {
    if (movieId) {
        ActiveMovie.value = (await fetchMovieById(Number(movieId))) as Movie;
        await fetchActorsForMovie(ActiveMovie.value.movieID);
        ActiveDirector.value = (await fetchDirectorById(ActiveMovie.value.directorID!)) as Director;
        ActiveGenre.value = (await fetchGenreById(ActiveMovie.value.genreID!)) as Genre;
    }
});

const openPDF = (path?: string) => {
    if (path) {
        let endPDF = serverURL + path;
        let win = window.open(endPDF, '_blank');
        win!.focus();
    }
};
</script>

<template>
    <div :style="`background-image: url(${Background}); height: 100vh; overflow-y: auto;`">
        <v-container v-if="movieId" class="d-flex justify-center align-center h-100">
            <v-row class="mx-auto pa-5 sm:pa-8 md:pa-6">
                <v-col cols="12">
                    <v-card class="mx-auto" style="background: #b9b9b926;">
                        <v-card-text class="mt-3 backdrop-blur-2xl rounded">
                            <v-card class="mx-auto" max-width="800">
                                <v-img :cover="false" height="500px" style="object-fit: scale-down !important;" :src="`${serverURL}${ActiveMovie?.posterPath}`"></v-img>

                                <v-card-title>
                                    {{ ActiveMovie?.title }}
                                </v-card-title>

                                <v-card-subtitle>
                                    <p>
                                        Año de publicacion: {{ ActiveMovie?.releaseYear }}
                                    </p>
                                    <p>
                                        Genero: {{ ActiveGenre?.name }}
                                    </p>
                                    <p class="cursor-pointer" @click="openPDF(ActiveDirector?.biographyPath)">
                                        Director: {{ ActiveDirector?.firstName }} {{ ActiveDirector?.lastName }}
                                    </p>
                                </v-card-subtitle>

                                <v-card-actions>
                                    <v-btn @click="showActors = !showActors" color="orange-lighten-2"
                                        text="Reparto"></v-btn>

                                    <v-spacer></v-spacer>

                                    <v-btn :icon="showActors ? 'mdi-chevron-up' : 'mdi-chevron-down'"
                                        @click="showActors = !showActors"></v-btn>
                                </v-card-actions>

                                <v-expand-transition>
                                    <div v-show="showActors">
                                        <v-divider></v-divider>

                                        <v-card-text>
                                            <v-list>
                                                <v-list-item v-for="(item, i) in currentMovieActors" :key="i"
                                                    :value="item" color="primary" rounded="shaped">
                                                    <v-list-item-title v-text="`${item.firstName} ${item.lastName}`"
                                                        @click="openPDF(item.biographyPath)"></v-list-item-title>
                                                </v-list-item>
                                            </v-list>
                                        </v-card-text>
                                    </div>
                                </v-expand-transition>
                            </v-card>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </div>

</template>

<style scoped>
.backdrop-blur-2xl {
    backdrop-filter: blur(20px);
}
</style>