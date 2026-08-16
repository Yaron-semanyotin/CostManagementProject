<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="CostWise.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="he" dir="rtl">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>הרשמה - Tamhiro</title>

    <script src="<%: ResolveUrl("~/Scripts/theme.js") %>"></script>

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.rtl.min.css"
        integrity="sha384-CfCrinSRH2IR6a4e6fy2q6ioOX7O6Mtm1L9vRvFZ1trBncWmMePhzvafv7oIcWiW" crossorigin="anonymous" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Content/Site.css") %>" />
</head>
<body class="app-auth-body">
    <form id="form1" runat="server">
        <div class="min-vh-100 d-flex flex-column">
            <header class="navbar navbar-dark app-auth-header">
                <div class="container-fluid">
                    <span class="navbar-brand mb-0 fw-semibold">Tamhiro</span>
                </div>
            </header>

            <main class="container flex-grow-1 d-flex align-items-center py-5">
                <div class="row justify-content-center w-100">
                    <div class="col-12 col-sm-10 col-md-8 col-lg-6 col-xl-5">
                        <section class="card shadow-sm app-auth-card">
                            <div class="card-body p-4 p-md-5">
                                <div class="text-center mb-4">
                                    <h1 class="h3 mb-1">Tamhiro</h1>
                                    <p class="text-secondary mb-4">ניהול עלויות ותמחור חכם לעסק שלך</p>
                                    <h2 class="h4 mb-0">הרשמה</h2>
                                </div>

                                <div class="mb-3">
                                    <asp:Label ID="BusinessNameLabel" runat="server" AssociatedControlID="BusinessNameTextBox"
                                        Text="שם העסק" CssClass="form-label" />
                                    <asp:TextBox ID="BusinessNameTextBox" runat="server" MaxLength="150" CssClass="form-control" />
                                </div>

                                <div class="mb-3">
                                    <asp:Label ID="UsernameLabel" runat="server" AssociatedControlID="UsernameTextBox"
                                        Text="שם משתמש" CssClass="form-label" />
                                    <asp:TextBox ID="UsernameTextBox" runat="server" MaxLength="50"
                                        CssClass="form-control" autocomplete="username" />
                                </div>

                                <div class="mb-3">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="PasswordTextBox"
                                        Text="סיסמה" CssClass="form-label" />
                                    <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password"
                                        MaxLength="128" CssClass="form-control" autocomplete="new-password" />
                                </div>

                                <div class="mb-3">
                                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPasswordTextBox"
                                        Text="אימות סיסמה" CssClass="form-label" />
                                    <asp:TextBox ID="ConfirmPasswordTextBox" runat="server" TextMode="Password"
                                        MaxLength="128" CssClass="form-control" autocomplete="new-password" />
                                </div>

                                <asp:Button ID="RegisterButton" runat="server" Text="הרשמה"
                                    CssClass="btn btn-primary w-100" OnClick="RegisterButton_Click" />

                                <asp:Label ID="RegistrationResultLabel" runat="server"
                                    CssClass="d-block text-center mt-3" role="status" />

                                <div class="text-center mt-4">
                                    <span class="text-secondary">כבר יש לך חשבון?</span>
                                    <a href="<%: ResolveUrl("~/Login.aspx") %>"
                                        class="btn btn-link p-0 fw-semibold text-decoration-none">התחבר</a>
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </main>
        </div>

        <div class="form-check form-switch m-0 app-theme-switch">
            <input type="checkbox" id="ThemeToggleButton" class="form-check-input app-theme-switch-input"
                role="switch" aria-label="הפעל מצב כהה" title="הפעל מצב כהה" />
            <label class="form-check-label app-theme-switch-label" for="ThemeToggleButton">
                <i id="ThemeToggleIcon" class="bi bi-moon-stars" aria-hidden="true"></i>
            </label>
        </div>
    </form>
</body>
</html>