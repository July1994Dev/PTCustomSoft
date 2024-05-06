<script setup lang="ts">
import { ref } from 'vue';
import { exportToExcel } from '../../utils/excelUtils';

const props = defineProps({
    headers: Array,
    items: Array,
    editAction: Function,
    deleteAction: Function,
    tableName: String,
    tableIcon: String,
    viewAction: Function
});

const search = ref<string>("");

const handleEdit = (item: any) => {
    if (props.editAction) {
        props.editAction(item);
    }
};

const handleDelete = (item: any) => {
    if (props.deleteAction) {
        props.deleteAction(item);
    }
};

const handleExport = () => {
    exportToExcel(props.items ?? [], props.tableName ?? "").then(() => console.log('Archivo Excel creado con éxito.'));
};

const handleView = (item: any) => {
    if (props.viewAction) {
        props.viewAction(item);
    }
};
</script>

<template>
    <v-card flat>
        <v-card-title class="d-flex align-center">
            <div class="w-100 d-flex flex-wrap">
                <div class="flex-1-0">
                    <v-icon :icon="props.tableIcon"></v-icon> &nbsp;
                    {{ props.tableName }}
                    <v-btn size="small" color="primary" @click="handleExport">
                        <v-icon>mdi-file-excel</v-icon>

                    </v-btn>
                </div>
                <v-text-field v-model="search" density="compact" label="Search" prepend-inner-icon="mdi-magnify"
                    variant="outlined" single-line hide-details></v-text-field>
            </div>
        </v-card-title>
        <v-divider></v-divider>
        <v-data-table :headers="props.headers as any[]" :items="props.items" :search="search">
            <template v-slot:[`item.actions`]="{ item }">
                <div class="d-flex flex-row ga-4 justify-center">
                    <v-btn size="small" color="blue" @click="handleEdit(item)">
                        <v-icon>mdi-pencil</v-icon>
                        Editar
                    </v-btn>
                    <v-btn size="small" color="red" @click="handleDelete(item)">
                        <v-icon>mdi-delete</v-icon>
                        Eliminar
                    </v-btn>
                    <v-btn v-if="viewAction" size="small" color="green" @click="handleView(item)">
                        <v-icon>mdi-eye</v-icon>
                        Visualizar
                    </v-btn>
                </div>
            </template>
        </v-data-table>
    </v-card>
</template>
