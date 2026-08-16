(function () {
    "use strict";

    var storageKey = "costwise-theme";

    function getStoredTheme() {
        try {
            return window.localStorage.getItem(storageKey);
        }
        catch (error) {
            return null;
        }
    }

    function storeTheme(theme) {
        try {
            window.localStorage.setItem(storageKey, theme);
        }
        catch (error) {
            // שמירת Theme היא שיפור תצוגה בלבד.
        }
    }

    function normalizeTheme(theme) {
        return theme === "dark" ? "dark" : "light";
    }

    function applyTheme(theme, notify) {
        document.documentElement.setAttribute("data-bs-theme", theme);

        if (notify && typeof window.CustomEvent === "function") {
            document.dispatchEvent(new CustomEvent(
                "costwise:themechange",
                { detail: { theme: theme } }
            ));
        }
    }

    function updateSwitch(theme) {
        var button = document.getElementById("ThemeToggleButton");
        var icon = document.getElementById("ThemeToggleIcon");

        if (!button || !icon) {
            return;
        }

        var isDark = theme === "dark";
        var label = isDark ? "הפעל מצב בהיר" : "הפעל מצב כהה";

        button.checked = isDark;
        button.setAttribute("aria-label", label);
        button.setAttribute("title", label);
        icon.className = isDark ? "bi bi-sun" : "bi bi-moon-stars";
    }

    var initialTheme = normalizeTheme(getStoredTheme());
    applyTheme(initialTheme, false);

    document.addEventListener("DOMContentLoaded", function () {
        var button = document.getElementById("ThemeToggleButton");

        if (!button) {
            return;
        }

        updateSwitch(initialTheme);

        button.addEventListener("change", function () {
            var nextTheme = button.checked ? "dark" : "light";

            applyTheme(nextTheme, true);
            storeTheme(nextTheme);
            updateSwitch(nextTheme);
        });
    });
})();