(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        var toggleButton = document.getElementById("ToggleInstructionsButton");
        var panel = document.getElementById("InstructionsEditorPanel");
        var textBox = document.getElementById("InstructionsHtmlTextBox");

        if (!toggleButton || !panel || !textBox) {
            return;
        }

        function isDarkTheme() {
            return document.documentElement.getAttribute("data-bs-theme") === "dark";
        }

        function initializeEditor() {
            if (typeof tinymce === "undefined"
                || tinymce.get("InstructionsHtmlTextBox")) {
                return;
            }

            var darkTheme = isDarkTheme();

            tinymce.init({
                selector: "#InstructionsHtmlTextBox",
                license_key: "gpl",
                height: 320,
                menubar: false,
                directionality: "rtl",
                skin: darkTheme ? "oxide-dark" : "oxide",
                content_css: darkTheme ? "dark" : "default",
                plugins: "lists",
                toolbar: "undo redo | blocks | bold italic | bullist numlist | removeformat",
                block_formats: "פסקה=p; כותרת=h3; כותרת משנה=h4",
                valid_elements: "p,br,strong/b,em/i,ul,ol,li,h3,h4,blockquote",
                xss_sanitization: true,
                content_style: "body { direction: rtl; text-align: right; font-family: system-ui, sans-serif; }",
                setup: function (editor) {
                    editor.on("change input undo redo", function () {
                        editor.save();
                    });
                }
            });
        }

        function getClosedButtonText() {
            return textBox.value.trim().length > 0
                ? "הצג הוראות הכנה"
                : "הוסף הוראות הכנה";
        }

        function openEditor() {
            panel.hidden = false;
            toggleButton.textContent = "מזער הוראות הכנה";
            toggleButton.setAttribute("aria-expanded", "true");
            initializeEditor();
        }

        function closeEditor() {
            var editor = typeof tinymce !== "undefined"
                ? tinymce.get("InstructionsHtmlTextBox")
                : null;

            if (editor) {
                editor.save();
            }

            panel.hidden = true;
            toggleButton.textContent = getClosedButtonText();
            toggleButton.setAttribute("aria-expanded", "false");
        }

        function reloadEditorTheme() {
            if (typeof tinymce === "undefined" || panel.hidden) {
                return;
            }

            var editor = tinymce.get("InstructionsHtmlTextBox");

            if (editor) {
                editor.save();
                editor.remove();
            }

            initializeEditor();
        }

        toggleButton.addEventListener("click", function () {
            if (panel.hidden) {
                openEditor();
            }
            else {
                closeEditor();
            }
        });

        document.addEventListener("costwise:themechange", reloadEditorTheme);

        toggleButton.textContent = getClosedButtonText();

        if (textBox.value.trim().length > 0) {
            openEditor();
        }
    });
})();