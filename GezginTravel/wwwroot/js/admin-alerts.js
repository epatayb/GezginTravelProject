window.AdminAlerts = {
    toast: function (icon, title) {
        if (!title) return;

        Swal.fire({
            toast: true,
            position: "top-end",
            icon: icon,
            title: title,
            showConfirmButton: false,
            timer: 2600,
            timerProgressBar: true
        });
    }
};

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("form.js-confirm-form").forEach(function (form) {
        form.addEventListener("submit", function (event) {
            if (form.dataset.confirmed === "true") {
                return;
            }

            if (window.jQuery && $(form).data("validator") && !$(form).valid()) {
                return;
            }

            event.preventDefault();

            const title = form.dataset.confirmTitle || "Bu işlemi onaylıyor musunuz?";
            const text = form.dataset.confirmText || "Bu işlemden sonra değişiklik uygulanacaktır.";
            const icon = form.dataset.confirmIcon || "warning";
            const confirmButtonText = form.dataset.confirmButton || "Evet, onayla";
            const cancelButtonText = form.dataset.cancelButton || "Vazgeç";

            Swal.fire({
                title: title,
                text: text,
                icon: icon,
                showCancelButton: true,
                confirmButtonText: confirmButtonText,
                cancelButtonText: cancelButtonText,
                reverseButtons: true,
                buttonsStyling: false,
                customClass: {
                    confirmButton: "px-5 py-2.5 rounded-full bg-primary text-on-primary font-bold mx-1",
                    cancelButton: "px-5 py-2.5 rounded-full bg-surface border border-outline-variant text-on-surface-variant font-bold mx-1"
                }
            }).then(function (result) {
                if (result.isConfirmed) {
                    form.dataset.confirmed = "true";
                    form.submit();
                }
            });
        });
    });
});