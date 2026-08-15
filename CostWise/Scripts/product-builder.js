(function () {
    "use strict";

    var productBuilderCacheKey = "CostWise.ProductBuilderData.v3";
    var productBuilderLoadPromise = null;
    var productBuilderData = null;
    var shouldClearProductBuilderCache = document.body !== null && document.body.getAttribute("data-clear-product-builder-cache") === "true";
    if (shouldClearProductBuilderCache) {
        try {
            window.sessionStorage.removeItem(productBuilderCacheKey);
        }
        catch (error) {
            // המטמון הוא שיפור ביצועים בלבד.
        }
        return;
    }
    var pageElement = document.getElementById("ProductBuilderPage");
    if (pageElement === null) {
        return;
    }
    var productBuilderEndpoint = pageElement.getAttribute("data-builder-endpoint");
    var ingredientCostPreviewEndpoint = pageElement.getAttribute("data-cost-preview-endpoint");
    var loginPageUrl = pageElement.getAttribute("data-login-url");
    var resultLabelId = pageElement.getAttribute("data-result-label-id");
    var recipeJsonFieldId = pageElement.getAttribute("data-recipe-json-field-id");
    var editingProductId = parseInt(pageElement.getAttribute("data-editing-product-id"), 10);
    var isEditingProduct = !isNaN(editingProductId) && editingProductId > 0;
    var recipeJsonField = recipeJsonFieldId ? document.getElementById(recipeJsonFieldId) : null;
    var shouldClearBuilderDataCache = pageElement.getAttribute("data-clear-builder-cache") === "true";
    var addRecipeIngredientButton = document.getElementById("AddRecipeIngredientButton");
    var addProductButton = document.getElementById("AddProductButton");
    var recipeIngredientRowsElement = document.getElementById("RecipeIngredientRows");
    var recipeIngredientRowTemplate = document.getElementById("RecipeIngredientRowTemplate");

    var recipeIngredientRowSequence = 0;
    function createRecipeIngredientRow() {
        if (recipeIngredientRowTemplate === null ||
            recipeIngredientRowsElement === null) {
            throw new Error("Recipe ingredient row elements are missing.");
        }
        var templateRow = recipeIngredientRowTemplate.content.firstElementChild;
        if (templateRow === null) {
            throw new Error("Recipe ingredient row template is empty.");
        }
        recipeIngredientRowSequence += 1;
        var rowNumber = recipeIngredientRowSequence;
        var rowElement = templateRow.cloneNode(true);
        var ingredientLabel = rowElement.querySelector('[data-role="ingredient-label"]');
        var ingredientSearch = rowElement.querySelector('[data-role="ingredient-search"]');
        var ingredientId = rowElement.querySelector('[data-role="ingredient-id"]');
        var ingredientSuggestions = rowElement.querySelector('[data-role="ingredient-suggestions"]');
        var ingredientError = rowElement.querySelector('[data-role="ingredient-error"]');
        var ingredientCostLabel = rowElement.querySelector('[data-role="ingredient-cost-label"]');
        var ingredientCost = rowElement.querySelector('[data-role="ingredient-cost"]');
        var quantityLabel = rowElement.querySelector('[data-role="quantity-label"]');
        var recipeQuantity = rowElement.querySelector('[data-role="recipe-quantity"]');
        var unitLabel = rowElement.querySelector('[data-role="unit-label"]');
        var recipeUnit = rowElement.querySelector('[data-role="recipe-unit"]');
        var removeButton = rowElement.querySelector('[data-role="remove-row"]');
        ingredientSearch.id = "RecipeIngredientSearch_" + rowNumber;
        ingredientId.id = "RecipeIngredientId_" + rowNumber;
        ingredientSuggestions.id = "RecipeIngredientSuggestions_" + rowNumber;
        ingredientCost.id = "RecipeIngredientCost_" + rowNumber;
        recipeQuantity.id = "RecipeIngredientQuantity_" + rowNumber;
        recipeUnit.id = "RecipeIngredientUnit_" + rowNumber;
        ingredientLabel.htmlFor = ingredientSearch.id;
        ingredientCostLabel.htmlFor = ingredientCost.id;
        quantityLabel.htmlFor = recipeQuantity.id;
        unitLabel.htmlFor = recipeUnit.id;
        ingredientSearch.setAttribute("aria-controls", ingredientSuggestions.id);
        ingredientSearch.addEventListener("input",
            function () {
                ingredientId.value = "";
                ingredientSearch.classList.remove("is-invalid");
                ingredientError.textContent = "";
                ingredientCost.value = "";
                ingredientCost.disabled = true;
                ingredientCost.setAttribute("data-manually-edited", "false");
                recipeUnit.value = "";
                recipeUnit.disabled = true;
                while (recipeUnit.options.length > 1) {
                    recipeUnit.remove(1);
                }
                var suggestions = getIngredientSuggestions(ingredientSearch.value);
                renderIngredientSuggestions(rowElement, suggestions);
            });
        ingredientSearch.addEventListener("keydown",
            function (event) {
                handleIngredientSearchKeyDown(event, rowElement);
            });
        recipeQuantity.addEventListener("change",
            function () {
                requestIngredientCostPreviewForRow(rowElement);
            });
        recipeUnit.addEventListener("change",
            function () {
                requestIngredientCostPreviewForRow(
                    rowElement);
            });

        ingredientCost.addEventListener("input",
            function () {
                ingredientCost.setAttribute(
                    "data-manually-edited",
                    "true");
            });
        removeButton.setAttribute("aria-label", "הסר שורת רכיב " + rowNumber);
        removeButton.addEventListener("click", function () { rowElement.remove(); updateRecipeIngredientRemoveButtons(); });
        recipeIngredientRowsElement.appendChild(rowElement);
        updateRecipeIngredientRemoveButtons();
        return rowElement;
    }
    function updateRecipeIngredientRemoveButtons() {
        if (recipeIngredientRowsElement === null) {
            return;
        }
        var rows = recipeIngredientRowsElement.querySelectorAll(".recipe-ingredient-row");
        for (var index = 0; index < rows.length; index += 1) {
            var removeButton = rows[index].querySelector('[data-role="remove-row"]');
            if (removeButton !== null) {
                removeButton.disabled = rows.length <= 1;
            }
        }
    }
    function isIngredientSelectedInAnotherRow(currentRow, ingredientId) {
        if (recipeIngredientRowsElement === null) {
            return false;
        }
        var rows = recipeIngredientRowsElement.querySelectorAll(".recipe-ingredient-row");

        for (var index = 0; index < rows.length; index += 1) {
            if (rows[index] === currentRow) {
                continue;
            }
            var ingredientIdInput = rows[index].querySelector('[data-role="ingredient-id"]');

            if (Number(ingredientIdInput.value) === ingredientId) {
                return true;
            }
        }
        return false;
    }
    function setActiveIngredientSuggestion(rowElement, activeIndex) {
        var ingredientSearch = rowElement.querySelector('[data-role="ingredient-search"]');
        var suggestionsElement = rowElement.querySelector('[data-role="ingredient-suggestions"]');
        var options = suggestionsElement.querySelectorAll('[role="option"]');
        for (var index = 0; index < options.length; index += 1) {
            var isActive = index === activeIndex;
            options[index].classList.toggle("active", isActive);
            options[index].setAttribute("aria-selected", isActive ? "true" : "false");
        }
        rowElement.setAttribute("data-active-suggestion-index", String(activeIndex));
        if (activeIndex >= 0 && activeIndex < options.length) {
            ingredientSearch.setAttribute("aria-activedescendant", options[activeIndex].id);
            options[activeIndex].scrollIntoView({
                block: "nearest"
            });
        }
        else {
            ingredientSearch.removeAttribute("aria-activedescendant");
        }
    }
    function handleIngredientSearchKeyDown(event, rowElement) {
        var ingredientSearch = rowElement.querySelector('[data-role="ingredient-search"]');
        var suggestionsElement = rowElement.querySelector('[data-role="ingredient-suggestions"]');
        if (event.key === "Escape") {
            suggestionsElement.hidden = true;
            ingredientSearch.setAttribute("aria-expanded", "false");
            setActiveIngredientSuggestion(rowElement, -1);
            return;
        }
        if (suggestionsElement.hidden) {
            return;
        }
        var options = suggestionsElement.querySelectorAll('[role="option"]');
        if (options.length === 0) {
            return;
        }
        var activeIndex = parseInt(rowElement.getAttribute("data-active-suggestion-index"), 10);
        if (isNaN(activeIndex)) {
            activeIndex = -1;
        }
        if (event.key === "ArrowDown") {
            event.preventDefault();
            activeIndex = activeIndex >= options.length - 1 ? 0 : activeIndex + 1;
            setActiveIngredientSuggestion(rowElement, activeIndex);
        }
        else if (event.key === "ArrowUp") {
            event.preventDefault();
            activeIndex = activeIndex <= 0 ? options.length - 1 : activeIndex - 1;
            setActiveIngredientSuggestion(rowElement, activeIndex);
        }
        else if (event.key === "Enter" && activeIndex >= 0) {
            event.preventDefault();
            options[activeIndex].click();
        }
    }
    function normalizeIngredientSearchText(value) {
        return String(value || "").trim().toLocaleLowerCase("he-IL");
    }
    function parseDecimalInputValue(value) {
        var normalizedValue = String(value || "").trim().replace(",", ".");

        if (normalizedValue === "") {
            return NaN;
        }

        return Number(normalizedValue);
    }
    function readInitialRecipeIngredients() {
        if (recipeJsonField === null) {
            return [];
        }
        var serializedRecipe = recipeJsonField.value.trim();
        if (serializedRecipe === "") {
            return [];
        }
        var recipeIngredients;
        try {
            recipeIngredients = JSON.parse(serializedRecipe);
        }
        catch (error) {
            throw new Error("Initial recipe data is invalid.");
        }
        if (!Array.isArray(recipeIngredients)) {
            throw new Error("Initial recipe data must be an array.");
        }
        return recipeIngredients;
    }
    function setFieldValidation(inputElement, errorElement, errorMessage) {
        var hasError = Boolean(errorMessage);
        inputElement.classList.toggle("is-invalid", hasError);
        errorElement.textContent = hasError ? errorMessage : "";
    }
    function validateSelectedIngredients() {
        if (recipeIngredientRowsElement === null) {
            return false;
        }
        var rowElements = recipeIngredientRowsElement.querySelectorAll(".recipe-ingredient-row");
        var allRowsAreValid = true;
        for (var index = 0; index < rowElements.length; index += 1) {
            var rowElement = rowElements[index];
            var ingredientSearch = rowElement.querySelector('[data-role="ingredient-search"]');
            var ingredientIdInput = rowElement.querySelector('[data-role="ingredient-id"]');
            var ingredientError = rowElement.querySelector('[data-role="ingredient-error"]');
            var ingredientId = parseInt(ingredientIdInput.value, 10);
            var selectedIngredient = findIngredientById(ingredientId);
            var errorMessage = selectedIngredient === null ? "יש לבחור רכיב מרשימת ההשלמה." : "";
            setFieldValidation(ingredientSearch, ingredientError, errorMessage);

            if (errorMessage !== "") {
                allRowsAreValid = false;
            }
        }
        return allRowsAreValid;
    }
    function validateRecipeQuantities() {
        if (recipeIngredientRowsElement === null) {
            return false;
        }
        var rowElements = recipeIngredientRowsElement.querySelectorAll(".recipe-ingredient-row");
        var allRowsAreValid = true;
        for (var index = 0; index < rowElements.length; index += 1) {
            var rowElement = rowElements[index];
            var quantityInput = rowElement.querySelector('[data-role="recipe-quantity"]');
            var quantityError = rowElement.querySelector('[data-role="quantity-error"]');
            var quantity = parseDecimalInputValue(quantityInput.value);
            var errorMessage = !isFinite(quantity) || quantity <= 0 ? "יש להזין כמות גדולה מאפס." : "";
            setFieldValidation(quantityInput, quantityError, errorMessage);
            if (errorMessage !== "") {
                allRowsAreValid = false;
            }
        }
        return allRowsAreValid;
    }
    function areUnitsCompatible(
        packageUnit,
        recipeUnit) {
        if (packageUnit === null ||
            recipeUnit === null) {
            return false;
        }

        var packageFamily =
            String(packageUnit.UnitFamily)
                .toLowerCase();

        var recipeFamily =
            String(recipeUnit.UnitFamily)
                .toLowerCase();

        var sameFamily =
            packageFamily === recipeFamily;

        var liquidMeasuredByWeight =
            packageFamily === "volume" &&
            recipeFamily === "weight";

        return sameFamily ||
            liquidMeasuredByWeight;
    }
    function validateRecipeUnits() {
        if (recipeIngredientRowsElement === null) {
            return false;
        }
        var rowElements = recipeIngredientRowsElement.querySelectorAll(".recipe-ingredient-row");
        var allRowsAreValid = true;
        for (var index = 0; index < rowElements.length; index += 1) {
            var rowElement = rowElements[index];
            var ingredientIdInput = rowElement.querySelector('[data-role="ingredient-id"]');
            var unitSelect = rowElement.querySelector('[data-role="recipe-unit"]');
            var unitError = rowElement.querySelector('[data-role="unit-error"]');
            var ingredientId = parseInt(ingredientIdInput.value, 10);
            var measurementUnitId = parseInt(unitSelect.value, 10);
            var selectedIngredient = findIngredientById(ingredientId);
            var selectedUnit = findMeasurementUnitById(measurementUnitId);
            var packageUnit = selectedIngredient === null ? null : findMeasurementUnitById(Number(selectedIngredient.PackageUnitId));
            var unitIsValid = areUnitsCompatible(packageUnit, selectedUnit);
            var errorMessage = unitIsValid ? "" : "יש לבחור יחידת מידה מתאימה לרכיב.";
            setFieldValidation(unitSelect, unitError, errorMessage);
            if (!unitIsValid) {
                allRowsAreValid = false;
            }
        }
        return allRowsAreValid;
    }
    function validateIngredientCosts() {
        if (recipeIngredientRowsElement === null) {
            return false;
        }
        var rowElements = recipeIngredientRowsElement.querySelectorAll(".recipe-ingredient-row");
        var allRowsAreValid = true;
        for (var index = 0; index < rowElements.length; index += 1) {
            var rowElement = rowElements[index];
            var ingredientCostInput = rowElement.querySelector('[data-role="ingredient-cost"]');
            var ingredientCostError = rowElement.querySelector('[data-role="ingredient-cost-error"]');
            var ingredientCost = parseDecimalInputValue(ingredientCostInput.value);
            var errorMessage = "";
            if (ingredientCostInput.disabled) {
                errorMessage = "יש להשלים את נתוני הרכיב ולהמתין לחישוב המחיר.";
            }
            else if (!isFinite(ingredientCost) || ingredientCost < 0) {
                errorMessage = "יש להזין מחיר תקין שאינו שלילי.";
            }
            setFieldValidation(ingredientCostInput, ingredientCostError, errorMessage);
            if (errorMessage !== "") {
                allRowsAreValid = false;
            }
        }
        return allRowsAreValid;
    }
    function buildRecipeIngredientsPayload() {
        if (recipeIngredientRowsElement === null) {
            return [];
        }
        var rowElements = recipeIngredientRowsElement.querySelectorAll(".recipe-ingredient-row");
        var recipeIngredients = [];
        for (var index = 0; index < rowElements.length; index += 1) {
            var rowElement = rowElements[index];
            var ingredientIdInput = rowElement.querySelector('[data-role="ingredient-id"]');
            var quantityInput = rowElement.querySelector('[data-role="recipe-quantity"]');
            var unitSelect = rowElement.querySelector('[data-role="recipe-unit"]');
            var ingredientCostInput = rowElement.querySelector('[data-role="ingredient-cost"]');
            var wasCostManuallyEdited = ingredientCostInput.getAttribute("data-manually-edited") === "true";
            recipeIngredients.push({
                IngredientId: parseInt(ingredientIdInput.value, 10),
                Quantity: parseDecimalInputValue(quantityInput.value),
                MeasurementUnitId: parseInt(unitSelect.value, 10),
                ManualIngredientCostOverride: wasCostManuallyEdited ? parseDecimalInputValue(ingredientCostInput.value) : null
            });
        }
        return recipeIngredients;
    }
    function prepareRecipeIngredientsForSubmit() {
        var ingredientsAreValid = validateSelectedIngredients();
        var quantitiesAreValid = validateRecipeQuantities();
        var unitsAreValid = validateRecipeUnits();
        var costsAreValid = validateIngredientCosts();
        var allFieldsAreValid = ingredientsAreValid && quantitiesAreValid && unitsAreValid && costsAreValid;
        if (!allFieldsAreValid || recipeJsonField === null) {
            if (recipeJsonField !== null) {
                recipeJsonField.value = "";
            }
            return false;
        }
        var recipeIngredients = buildRecipeIngredientsPayload();
        if (recipeIngredients.length === 0) {
            recipeJsonField.value = "";
            return false;
        }
        recipeJsonField.value = JSON.stringify(recipeIngredients);
        return true;
    }
    function getIngredientSuggestions(searchText) {
        if (productBuilderData === null) {
            return [];
        }
        var normalizedSearchText = normalizeIngredientSearchText(searchText);
        if (normalizedSearchText.length === 0) {
            return [];
        }
        var prefixMatches = [];
        var containsMatches = [];
        for (var index = 0; index < productBuilderData.Ingredients.length; index += 1) {
            var ingredient = productBuilderData.Ingredients[index];
            if (ingredient.IsActive !== true) {
                continue;
            }
            var normalizedIngredientName = normalizeIngredientSearchText(ingredient.IngredientName);
            if (normalizedIngredientName.indexOf(
                normalizedSearchText) === 0) {
                prefixMatches.push(ingredient);
            }
            else if (normalizedIngredientName.indexOf(normalizedSearchText) > -1) {
                containsMatches.push(ingredient);
            }
        }
        return prefixMatches.concat(containsMatches).slice(0, 10);
    }
    function findIngredientById(ingredientId) {
        if (productBuilderData === null) {
            return null;
        }

        for (var index = 0;
            index < productBuilderData.Ingredients.length;
            index += 1) {

            var ingredient =
                productBuilderData.Ingredients[index];

            if (Number(ingredient.IngredientId) ===
                ingredientId) {
                return ingredient;
            }
        }

        return null;
    }
    function findMeasurementUnitById(measurementUnitId) {
        if (productBuilderData === null) {
            return null;
        }
        for (var index = 0;
            index < productBuilderData.MeasurementUnits.length;
            index += 1) {
            var measurementUnit = productBuilderData.MeasurementUnits[index];

            if (Number(measurementUnit.MeasurementUnitId) === measurementUnitId) {
                return measurementUnit;
            }
        }
        return null;
    }
    function populateCompatibleUnitsForRow(rowElement, ingredient) {
        var recipeUnit = rowElement.querySelector('[data-role="recipe-unit"]');
        while (recipeUnit.options.length > 1) {
            recipeUnit.remove(1);
        }
        var packageUnit = findMeasurementUnitById(Number(ingredient.PackageUnitId));
        if (packageUnit === null || productBuilderData === null) {
            recipeUnit.disabled = true;
            return;
        }
        for (var index = 0; index < productBuilderData.MeasurementUnits.length; index += 1) {
            var measurementUnit = productBuilderData.MeasurementUnits[index];
            if (areUnitsCompatible(packageUnit, measurementUnit)) {
                var option = document.createElement("option");
                option.value = String(measurementUnit.MeasurementUnitId);
                option.textContent = measurementUnit.UnitName;
                recipeUnit.appendChild(option);
            }
        }
        var defaultRecipeUnitId = Number(productBuilderData.DefaultRecipeMeasurementUnitId);
        var defaultRecipeUnit = findMeasurementUnitById(defaultRecipeUnitId);
        var selectedRecipeUnit = areUnitsCompatible(packageUnit, defaultRecipeUnit) ? defaultRecipeUnit : packageUnit;
        recipeUnit.value = String(selectedRecipeUnit.MeasurementUnitId);
        recipeUnit.disabled = false;
    }
    function formatDecimalForDisplay(value) {
        var numericValue = Number(value);
        if (isNaN(numericValue)) {
            return "";
        }
        return numericValue.toLocaleString(
            "he-IL",
            {
                useGrouping: false,
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
    }
    function loadIngredientCostPreview(rowElement) {
        var ingredientIdInput = rowElement.querySelector('[data-role="ingredient-id"]');
        var recipeQuantity = rowElement.querySelector('[data-role="recipe-quantity"]');
        var recipeUnit = rowElement.querySelector('[data-role="recipe-unit"]');
        var ingredientCost = rowElement.querySelector('[data-role="ingredient-cost"]');
        var ingredientId = parseInt(ingredientIdInput.value, 10);
        var normalizedQuantity = recipeQuantity.value.trim().replace(",", ".");
        var quantity = Number(normalizedQuantity);
        var measurementUnitId = parseInt(recipeUnit.value, 10);
        if (isNaN(ingredientId) || isNaN(quantity) || quantity <= 0 || isNaN(measurementUnitId)) {
            ingredientCost.value = "";
            ingredientCost.disabled = true;
            ingredientCost.setAttribute("data-manually-edited", "false");
            return Promise.resolve(null);
        }
        if (!ingredientCostPreviewEndpoint) {
            return Promise.reject(new Error("Ingredient cost preview endpoint is missing."));
        }
        var requestVersion = Number(rowElement.getAttribute("data-cost-request-version") || "0") + 1;
        rowElement.setAttribute("data-cost-request-version", String(requestVersion));
        ingredientCost.disabled = true;
        return fetch(
            ingredientCostPreviewEndpoint,
            {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    ProductId: isEditingProduct ? editingProductId : null,
                    IngredientId: ingredientId,
                    Quantity: quantity,
                    MeasurementUnitId: measurementUnitId
                })
            })
            .then(function (response) {
                if (response.status === 401) {
                    if (loginPageUrl) {
                        window.location.href = loginPageUrl;
                    }
                    throw new Error("Authentication is required.");
                }
                if (!response.ok) {
                    throw new Error("Ingredient cost could not be calculated.");
                }
                return response.json();
            })
            .then(function (data) {
                var currentRequestVersion = Number(rowElement.getAttribute("data-cost-request-version"));
                if (currentRequestVersion !== requestVersion) {
                    return null;
                }
                if (data === null || isNaN(Number(data.CalculatedCost))) {
                    throw new Error("Ingredient cost response is invalid.");
                }
                ingredientCost.value = formatDecimalForDisplay(data.CalculatedCost);
                ingredientCost.disabled = false;
                ingredientCost.setAttribute("data-manually-edited", "false");
                return Number(data.CalculatedCost);
            });
    }
    function requestIngredientCostPreviewForRow(rowElement) {
        var errorMessage = "לא ניתן לחשב את מחיר הרכיב. בדוק את הכמות והיחידה.";
        loadIngredientCostPreview(rowElement)
            .then(function (calculatedCost) {
                if (calculatedCost === null || !resultLabelId) {
                    return;
                }
                var resultLabel = document.getElementById(resultLabelId);
                if (resultLabel !== null && resultLabel.textContent === errorMessage) {
                    resultLabel.textContent = "";
                }
            })
            .catch(function () {
                var ingredientCost = rowElement.querySelector('[data-role="ingredient-cost"]');
                ingredientCost.value = "";
                ingredientCost.disabled = true;
                ingredientCost.setAttribute("data-manually-edited", "false");
                if (!resultLabelId) {
                    return;
                }
                var resultLabel = document.getElementById(resultLabelId);
                if (resultLabel !== null) {
                    resultLabel.textContent = errorMessage;
                }
            });
    }
    function selectIngredientForRow(rowElement, ingredientId) {
        var ingredient = findIngredientById(ingredientId);
        if (ingredient === null) {
            return;
        }
        var ingredientSearch = rowElement.querySelector('[data-role="ingredient-search"]');
        var ingredientIdInput = rowElement.querySelector('[data-role="ingredient-id"]');
        var suggestionsElement = rowElement.querySelector('[data-role="ingredient-suggestions"]');
        var ingredientCost = rowElement.querySelector('[data-role="ingredient-cost"]');
        var ingredientError = rowElement.querySelector('[data-role="ingredient-error"]');
        if (isIngredientSelectedInAnotherRow(rowElement, ingredientId)) {
            ingredientIdInput.value = "";
            ingredientSearch.classList.add("is-invalid");
            ingredientError.textContent = "הרכיב כבר נבחר בשורת מתכון אחרת.";
            suggestionsElement.hidden = true;
            ingredientSearch.setAttribute("aria-expanded", "false");
            return;
        }
        ingredientSearch.classList.remove("is-invalid");
        ingredientError.textContent = "";
        ingredientSearch.value = ingredient.IngredientName;
        ingredientIdInput.value = String(ingredient.IngredientId);
        ingredientCost.value = "";
        ingredientCost.disabled = true;

        ingredientCost.setAttribute("data-manually-edited", "false");
        populateCompatibleUnitsForRow(rowElement, ingredient);
        requestIngredientCostPreviewForRow(rowElement);
        suggestionsElement.hidden = true;
        ingredientSearch.setAttribute("aria-expanded", "false");
        ingredientSearch.removeAttribute("aria-activedescendant");
    }
    function populateRecipeIngredientRow(recipeIngredient) {
        if (recipeIngredient === null || typeof recipeIngredient !== "object") {
            throw new Error("Recipe ingredient data is invalid.");
        }
        var ingredientId = Number(recipeIngredient.IngredientId);
        var quantity = Number(recipeIngredient.Quantity);
        var measurementUnitId = Number(recipeIngredient.MeasurementUnitId);
        if (!isFinite(ingredientId) || ingredientId <= 0) {
            throw new Error("Recipe ingredient id is invalid.");
        }
        if (!isFinite(quantity) || quantity <= 0) {
            throw new Error("Recipe ingredient quantity is invalid.");
        }
        if (!isFinite(measurementUnitId) || measurementUnitId <= 0) {
            throw new Error("Recipe measurement unit is invalid.");
        }
        var ingredient = findIngredientById(ingredientId);
        var measurementUnit = findMeasurementUnitById(measurementUnitId);
        if (ingredient === null || measurementUnit === null) {
            throw new Error("Recipe ingredient data is unavailable.");
        }
        var rowElement = createRecipeIngredientRow();
        selectIngredientForRow(rowElement, ingredientId);
        var ingredientIdInput = rowElement.querySelector('[data-role="ingredient-id"]');
        if (Number(ingredientIdInput.value) !== ingredientId) {
            throw new Error("Recipe ingredient could not be selected.");
        }
        var quantityInput = rowElement.querySelector('[data-role="recipe-quantity"]');
        var unitSelect = rowElement.querySelector('[data-role="recipe-unit"]');
        var costInput = rowElement.querySelector('[data-role="ingredient-cost"]');
        quantityInput.value = String(quantity);
        unitSelect.value = String(measurementUnitId);
        if (Number(unitSelect.value) !== measurementUnitId) {
            throw new Error("Recipe measurement unit is incompatible.");
        }
        var manualCost = recipeIngredient.ManualIngredientCostOverride;
        if (manualCost !== null && manualCost !== undefined) {
            manualCost = Number(manualCost);
            if (!isFinite(manualCost) || manualCost < 0) {
                throw new Error("Manual ingredient cost is invalid.");
            }
            costInput.value = formatDecimalForDisplay(manualCost);
            costInput.disabled = false;
            costInput.setAttribute("data-manually-edited", "true");
        }
        else {
            requestIngredientCostPreviewForRow(rowElement);
        }
        return rowElement;
    }
    function populateInitialRecipeIngredientRows() {
        if (recipeIngredientRowsElement === null) {
            throw new Error("Recipe ingredient rows element is missing.");
        }
        var initialRecipeIngredients = readInitialRecipeIngredients();
        while (recipeIngredientRowsElement.firstChild !== null) {
            recipeIngredientRowsElement.removeChild(recipeIngredientRowsElement.firstChild);
        }
        if (initialRecipeIngredients.length === 0) {
            createRecipeIngredientRow();
            return;
        }
        try {
            for (var index = 0; index < initialRecipeIngredients.length; index += 1) {
                populateRecipeIngredientRow(initialRecipeIngredients[index]);
            }
        }
        catch (error) {
            while (recipeIngredientRowsElement.firstChild !== null) {
                recipeIngredientRowsElement.removeChild(recipeIngredientRowsElement.firstChild);
            }
            throw error;
        }
        updateRecipeIngredientRemoveButtons();
    }
    function renderIngredientSuggestions(rowElement, suggestions) {
        var ingredientSearch = rowElement.querySelector('[data-role="ingredient-search"]');
        var suggestionsElement = rowElement.querySelector('[data-role="ingredient-suggestions"]');
        setActiveIngredientSuggestion(rowElement, -1);
        while (suggestionsElement.firstChild !== null) {
            suggestionsElement.removeChild(suggestionsElement.firstChild);
        }
        if (suggestions.length === 0) {
            suggestionsElement.hidden = true;
            ingredientSearch.setAttribute("aria-expanded", "false");
            ingredientSearch.removeAttribute("aria-activedescendant");
            return;
        }
        for (var index = 0; index < suggestions.length; index += 1) {
            var ingredient = suggestions[index];
            var optionButton = document.createElement("button");
            optionButton.type = "button";
            optionButton.className = "list-group-item list-group-item-action";
            optionButton.setAttribute("role", "option");
            optionButton.setAttribute("aria-selected", "false");
            optionButton.id = suggestionsElement.id + "_Option_" + index;
            optionButton.setAttribute("data-ingredient-id", String(ingredient.IngredientId));
            optionButton.textContent = ingredient.IngredientName;
            optionButton.addEventListener("click",
                function (event) {
                    var selectedIngredientId = parseInt(event.currentTarget.getAttribute("data-ingredient-id"), 10);
                    if (!isNaN(selectedIngredientId)) {
                        selectIngredientForRow(rowElement, selectedIngredientId);
                    }
                });
            suggestionsElement.appendChild(optionButton);
        }
        suggestionsElement.hidden = false;
        ingredientSearch.setAttribute("aria-expanded", "true");
    }
    function isValidProductBuilderData(data) {
        return data !== null && typeof data === "object" &&
            Array.isArray(data.Ingredients) && Array.isArray(data.MeasurementUnits);
    }

    function clearProductBuilderData() {
        productBuilderLoadPromise = null;
        try {
            window.sessionStorage.removeItem(productBuilderCacheKey);
        }
        catch (error) {
            // המטמון הוא שיפור ביצועים בלבד.
        }
    }

    function readCachedProductBuilderData() {
        try {
            var cachedJson = window.sessionStorage.getItem(productBuilderCacheKey);
            if (!cachedJson) {
                return null;
            }
            var cachedData = JSON.parse(cachedJson);
            if (!isValidProductBuilderData(cachedData)) {
                clearProductBuilderData();
                return null;
            }
            return cachedData;
        }
        catch (error) {
            clearProductBuilderData();
            return null;
        }
    }

    function saveProductBuilderData(data) {
        try {
            window.sessionStorage.setItem(productBuilderCacheKey, JSON.stringify(data));
        }
        catch (error) {
            // הנתונים יישארו בזיכרון העמוד הנוכחי.
        }
    }

    function loadProductBuilderData() {
        if (!isEditingProduct) {
            var cachedData = readCachedProductBuilderData();
            if (cachedData !== null) {
                return Promise.resolve(cachedData);
            }
        }
        if (productBuilderLoadPromise !== null) {
            return productBuilderLoadPromise;
        }
        if (!productBuilderEndpoint) {
            return Promise.reject(new Error("Product builder endpoint is missing."));
        }
        var requestUrl = productBuilderEndpoint;
        if (isEditingProduct) {
            var querySeparator = requestUrl.indexOf("?") === -1 ? "?" : "&";
            requestUrl += querySeparator + "productId=" + encodeURIComponent(String(editingProductId));
        }
        productBuilderLoadPromise = fetch(requestUrl,
            {
                method: "GET",
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json"
                }
            })
            .then(function (response) {
                if (response.status === 401) {
                    if (loginPageUrl) {
                        window.location.href = loginPageUrl;
                    }
                    throw new Error("Authentication is required.");
                }
                if (!response.ok) {
                    throw new Error("Product builder data could not be loaded.");
                }
                return response.json();
            })
            .then(function (data) {
                if (!isValidProductBuilderData(data)) {
                    throw new Error("Product builder data is invalid.");
                }
                if (!isEditingProduct) {
                    saveProductBuilderData(data);
                }
                return data;
            })
            .catch(function (error) {
                productBuilderLoadPromise = null;
                throw error;
            });
        return productBuilderLoadPromise;
    }
    function displayLoadError() {
        if (!resultLabelId) {
            return;
        }
        var resultLabel = document.getElementById(resultLabelId);
        if (resultLabel !== null &&
            resultLabel.textContent.trim() === "") {
            resultLabel.textContent = "לא ניתן לטעון את נתוני הרכיבים. נסה לרענן את העמוד.";
        }
    }
    if (shouldClearBuilderDataCache) {
        clearProductBuilderData();
    }
    window.costWiseProductBuilderData = {
        load: loadProductBuilderData,
        clear: clearProductBuilderData
    };
    if (addProductButton !== null) {
        addProductButton.addEventListener("click",
            function (event) {
                var submissionIsReady = prepareRecipeIngredientsForSubmit();
                if (submissionIsReady) {
                    return;
                }
                event.preventDefault();
                if (resultLabelId) {
                    var resultLabel = document.getElementById(resultLabelId);
                    if (resultLabel !== null) {
                        resultLabel.textContent = "יש לתקן את פרטי רכיבי המתכון לפני יצירת המוצר.";
                    }
                }
                var firstInvalidField = pageElement.querySelector(".recipe-ingredient-row .is-invalid");
                if (firstInvalidField !== null) {
                    firstInvalidField.focus();
                }
            });
    }
    loadProductBuilderData()
        .then(function (data) {
            productBuilderData = data;
            if (addRecipeIngredientButton === null ||
                recipeIngredientRowsElement === null) {
                throw new Error("Recipe ingredient controls are missing.");
            }
            populateInitialRecipeIngredientRows();
            recipeIngredientRowsElement.setAttribute("aria-busy", "false");
            addRecipeIngredientButton.disabled = false;
            addRecipeIngredientButton.addEventListener("click", function () { createRecipeIngredientRow(); });
        })
        .catch(function () {
            if (recipeIngredientRowsElement !== null) {
                recipeIngredientRowsElement.setAttribute("aria-busy", "false");
            }
            displayLoadError();
        });
}());