import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useInterfaceStore = defineStore('interface', () => {
    const drawer = ref<boolean>(false);
    const rail = ref<boolean>(false);


    return { drawer, rail };
});