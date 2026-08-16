(function () {
    "use strict";

    var content;
    var storageKey;

    function saveScrollPosition() {
        if (!content) {
            return;
        }

        try {
            window.sessionStorage.setItem(
                storageKey,
                String(content.scrollTop)
            );
        }
        catch (error) {
            // שמירת מיקום הגלילה היא שיפור תצוגה בלבד.
        }
    }

    function restoreScrollPosition() {
        var storedPosition;

        try {
            storedPosition = window.sessionStorage.getItem(storageKey);
            window.sessionStorage.removeItem(storageKey);
        }
        catch (error) {
            return;
        }

        if (storedPosition === null) {
            return;
        }

        var position = parseInt(storedPosition, 10);

        if (isNaN(position) || position < 0) {
            return;
        }

        window.requestAnimationFrame(function () {
            window.requestAnimationFrame(function () {
                content.scrollTop = position;
            });
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        content = document.querySelector(".app-content");
        storageKey = "costwise-scroll:" + window.location.pathname.toLowerCase();

        if (!content) {
            return;
        }

        restoreScrollPosition();

        var form = document.getElementById("MainForm");

        if (form) {
            form.addEventListener("submit", saveScrollPosition);
        }

        document.addEventListener("click", function (event) {
            var link = event.target.closest("a");

            if (link && link.href && link.href.indexOf("__doPostBack") >= 0) {
                saveScrollPosition();
            }
        }, true);

        var originalPostBack = window.__doPostBack;

        if (typeof originalPostBack === "function") {
            window.__doPostBack = function () {
                saveScrollPosition();
                return originalPostBack.apply(this, arguments);
            };
        }
    });
})();