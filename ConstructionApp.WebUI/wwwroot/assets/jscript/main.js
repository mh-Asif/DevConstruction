<script>

document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("searchInput");
    const dropdownMenus = document.querySelectorAll(".dropdown-menu .submenu .dropdown-item");

    searchInput.addEventListener("keyup", function () {
        const filter = searchInput.value.toLowerCase();

        dropdownMenus.forEach(item => {
            const text = item.textContent.trim().toLowerCase();
            if (text.includes(filter)) {
                item.style.display = "block"; // Show matching items
            } else {
                item.style.display = "none"; // Hide non-matching items
            }
        });
    });
});

</script>