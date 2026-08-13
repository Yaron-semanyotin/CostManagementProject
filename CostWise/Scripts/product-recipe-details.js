(function () {
    "use strict";
    var detailsPanel = document.getElementById("ProductRecipeDetailsPanel");
    if (detailsPanel === null) {
        return;
    }
    var selectedRow = document.querySelector('tr[data-recipe-selected="true"]');
    if (selectedRow === null) {
        return;
    }
    var selectedButton = selectedRow.querySelector('[data-role="open-recipe"]');
    if (selectedButton === null) {
        return;
    }
    var detailsRow = document.createElement("tr");
    detailsRow.className = "product-recipe-details-row";
    var detailsCell = document.createElement("td");
    detailsCell.colSpan = selectedRow.cells.length;
    detailsCell.className = "p-0 border-0";
    detailsRow.appendChild(detailsCell);
    selectedRow.parentNode.insertBefore(detailsRow, selectedRow.nextSibling);
    detailsCell.appendChild(detailsPanel);
    selectedButton.textContent = "סגור מתכון";
}());