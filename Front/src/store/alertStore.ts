import { defineStore } from 'pinia';
import Swal from 'sweetalert2';

export const useAlertStore = defineStore('alert', {
    actions: {
        showToast(title: string, description: string, type: 'success' | 'error' | 'warning' | 'info') {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: type,
                title: title,
                text: description,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        },
        showAlert(title: string, description: string, type: 'success' | 'error' | 'warning' | 'info') {
            Swal.fire({
                icon: type,
                title: title,
                text: description,
                confirmButtonText: 'Aceptar'
            });
        },
        showConfirm(title: string, description: string, onConfirm: () => void) {
            Swal.fire({
                title: title,
                text: description,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Confirmar'
            }).then((result) => {
                if (result.isConfirmed) {
                    onConfirm();
                }
            });
        }
    }
});
