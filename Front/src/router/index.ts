import { createRouter, createWebHistory } from 'vue-router';
import Login from '../views/Login.vue';
import DashboardLayout from '../components/layout/DashboardLayout.vue';
import Actors from '../views/Actors.vue';
import Directors from '../views/Directors.vue';
import Genres from '../views/Genres.vue';
import Movies from '../views/Movies.vue';
import Viewer from '../views/Viewer.vue';

const routes = [
    {
        path: '/',
        name: 'Login',
        component: Login,
        meta: { requiresAuth: false }
    },
    {
        path: '/Viewer/:id?',
        name: 'Viewer',
        component: Viewer,
        meta: { requiresAuth: false }
    },
    {
        path: '/Dashboard',
        component: DashboardLayout,
        children: [
            {
                path: 'Actors',
                name: 'Actors',
                component: Actors,
                meta: { requiresAuth: true }
            },
            {
                path: 'Directors',
                name: 'Directors',
                component: Directors,
                meta: { requiresAuth: true }
            },
            {
                path: 'Genres',
                name: 'Genres',
                component: Genres,
                meta: { requiresAuth: true }
            },
            {
                path: 'Movies',
                name: 'Movies',
                component: Movies,
                meta: { requiresAuth: true }
            },
        ]
    },
];

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes,
});

export default router;
