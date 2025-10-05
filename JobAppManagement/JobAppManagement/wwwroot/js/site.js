window.scrollRowIntoView = (rowId) => {
    const row = document.getElementById("row-" + rowId);
    if (row) {
        row.scrollIntoView({ behavior: "smooth", block: "center" });
    }
};
