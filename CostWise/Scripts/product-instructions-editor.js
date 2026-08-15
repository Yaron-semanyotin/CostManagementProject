(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        var toggleButton =
            document.getElementById("ToggleInstructionsButton");

        var instructionsPanel =
            document.getElementById("InstructionsEditorPanel");

        var instructionsTextBox =
            document.getElementById("InstructionsHtmlTextBox");

        if (!toggleButton || !instructionsPanel || !instructionsTextBox) {
            return;
        }

        function initializeEditor() {
            if (typeof tinymce === "undefined") {
                return;
            }

            if (tinymce.get("InstructionsHtmlTextBox")) {
                return;
            }

            tinymce.init({
                selector: "#InstructionsHtmlTextBox",
                license_key: "gpl",
                height: 320,
                menubar: false,
                directionality: "rtl",
                plugins: "lists",
                toolbar: "undo redo | blocks | bold italic | bullist numlist | removeformat",
                block_formats: "פסקה=p; כותרת=h3; כותרת משנה=h4",
                valid_elements: "p,br,strong/b,em/i,ul,ol,li,h3,h4,blockquote",
                xss_sanitization: true,
                content_style:
                    "body { direction: rtl; text-align: right; font-family: Arial, sans-serif; }",

                setup: function (editor) {
                    editor.on("change input undo redo", function () {
                        editor.save();
                    });
                }
            });
        }

        function getClosedButtonText() {
            if (instructionsTextBox.value.trim().length > 0) {
                return "הצג הוראות הכנה";
            }

            return "הוסף הוראות הכנה";
        }

        function openInstructionsEditor() {
            instructionsPanel.hidden = false;
            toggleButton.textContent = "מזער הוראות הכנה";
            toggleButton.setAttribute("aria-expanded", "true");

            initializeEditor();
        }

        function closeInstructionsEditor() {
            if (typeof tinymce !== "undefined") {
                var editor = tinymce.get("InstructionsHtmlTextBox");

                if (editor) {
                    editor.save();
                }
            }

            instructionsPanel.hidden = true;
            toggleButton.textContent = getClosedButtonText();
            toggleButton.setAttribute("aria-expanded", "false");
        }

        toggleButton.addEventListener("click", function () {
            if (instructionsPanel.hidden) {
                openInstructionsEditor();
                return;
            }

            closeInstructionsEditor();
        });

        toggleButton.textContent = getClosedButtonText();

        if (instructionsTextBox.value.trim().length > 0) {
            openInstructionsEditor();
        }
    });
}());