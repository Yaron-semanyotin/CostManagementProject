(function () {
    "use strict";

    const passwordTextBox = document.getElementById("PasswordTextBox");
    const togglePasswordButton = document.getElementById("TogglePasswordButton");
    const passwordHiddenIcon = document.getElementById("PasswordHiddenIcon");
    const passwordVisibleIcon = document.getElementById("PasswordVisibleIcon");

    if (!passwordTextBox || !togglePasswordButton || !passwordHiddenIcon || !passwordVisibleIcon) {
        return;
    }

    togglePasswordButton.addEventListener("click", function () {
        const shouldShowPassword = passwordTextBox.type === "password";

        passwordTextBox.type = shouldShowPassword ? "text" : "password";
        passwordHiddenIcon.classList.toggle("d-none", shouldShowPassword);
        passwordVisibleIcon.classList.toggle("d-none", !shouldShowPassword);
        togglePasswordButton.setAttribute("aria-pressed", shouldShowPassword ? "true" : "false");
        togglePasswordButton.setAttribute("aria-label", shouldShowPassword ? "הסתר סיסמה" : "הצג סיסמה");
        togglePasswordButton.setAttribute("title", shouldShowPassword ? "הסתר סיסמה" : "הצג סיסמה");
    });
})();