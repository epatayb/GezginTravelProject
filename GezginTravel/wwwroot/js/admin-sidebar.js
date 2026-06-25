
document.addEventListener("DOMContentLoaded", function () {
    const menuBtn = document.getElementById("menuBtn");
    const sidebar = document.getElementById("sidebar");

    if (menuBtn && sidebar) {
        // Butona tıklandığında menüyü aç/kapat
        menuBtn.addEventListener("click", function (e) {
            e.stopPropagation(); // Tıklama olayının yukarı tırmanmasını engeller
            sidebar.classList.toggle("-translate-x-full");
        });

        // Menü dışındaki boş bir alana tıklandığında menüyü otomatik kapat
        document.addEventListener("click", function (e) {
            if (!sidebar.contains(e.target) && !menuBtn.contains(e.target)) {
                sidebar.classList.add("-translate-x-full");
            }
        });
    }
});
