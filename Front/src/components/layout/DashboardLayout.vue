<script setup lang="ts">
import { computed } from 'vue';
import { useInterfaceStore } from '../../store/interfaceStore';
import { storeToRefs } from 'pinia';
import { useRoute } from 'vue-router';
import router from '../../router';
import { setCookie, getCookie } from '../../utils/cookieUtils';

const route = useRoute();
const { drawer } = storeToRefs(useInterfaceStore());

const activeModule = computed(() => {
  let hasToken = getCookie("token");

  if (!hasToken) {
    logout();
  }

  switch (route.name) {
    case 'Actors':
      return 'Actores';
    case 'Directors':
      return 'Directores';
    case 'Genres':
      return 'Géneros';
    case 'Movies':
      return 'Películas';
    case 'Users':
      return 'Usuarios';
    default:
      return 'Inicio';
  }
});

const items = [
  { text: 'Directores', icon: 'mdi-chair-school', to: '/Dashboard/Directors' },
  { text: 'Actores', icon: 'mdi-account-convert', to: '/Dashboard/Actors' },
  { text: 'Generos', icon: 'mdi-filmstrip-box-multiple', to: '/Dashboard/Genres' },
  { text: 'Peliculas', icon: 'mdi-movie-open', to: '/Dashboard/Movies' },
];

const navigate = (to: string) => {
  router.push(to);
};

const logout = () => {
  setCookie('token', '', -99);
  router.push({ name: "Login" });
};
</script>

<template>
  <v-card>
    <v-layout>
      <v-app-bar color="primary" prominent>
        <v-app-bar-nav-icon variant="text" @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
        <v-toolbar-title>{{ activeModule }}</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon="mdi-logout" variant="text" @click="logout"></v-btn>
      </v-app-bar>
      <v-navigation-drawer v-model="drawer" temporary>
        <v-list>
          <v-list-item v-for="(item, i) in items" :key="i" :value="item" color="primary" rounded="shaped"
            @click="navigate(item.to)">
            <template v-slot:prepend>
              <v-icon :icon="item.icon"></v-icon>
            </template>
            <v-list-item-title v-text="item.text"></v-list-item-title>
          </v-list-item>
        </v-list>
      </v-navigation-drawer>
      <v-main class="main-content">
        <div class="pa-8">
          <RouterView />
        </div>
      </v-main>
    </v-layout>
  </v-card>
</template>

<style scoped>
.main-content {
  height: 100vh;
  overflow-y: auto;
}
</style>