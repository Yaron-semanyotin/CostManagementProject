<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="CostWise.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="he" dir="rtl">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>התחברות</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.rtl.min.css" integrity="sha384-CfCrinSRH2IR6a4e6fy2q6ioOX7O6Mtm1L9vRvFZ1trBncWmMePhzvafv7oIcWiW" crossorigin="anonymous" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />
</head>
<body class="bg-light" data-clear-product-builder-cache="true">
    <form id="form1" runat="server">
        <div class="min-vh-100 d-flex flex-column">
            <header class="navbar navbar-dark bg-dark shadow-sm">
                <div class="container-fluid">
                    <span class="navbar-brand mb-0 h1">CostWise</span>
                </div>
            </header>

            <main class="container flex-grow-1 d-flex align-items-center py-5">
                <div class="row justify-content-center w-100">
                    <div class="col-12 col-sm-10 col-md-8 col-lg-6 col-xl-5">
                        <section class="card border-0 shadow-sm">
                            <div class="card-body p-4 p-md-5">
                                <h1 class="h3 text-center mb-2">התחברות</h1>
                                <p class="text-secondary text-center mb-4">התחבר לחשבון שלך</p>

                                <div class="mb-3">
                                    <asp:Label ID="UsernameLabel" runat="server" AssociatedControlID="UsernameTextBox" Text="שם משתמש" CssClass="form-label" />
                                    <asp:TextBox ID="UsernameTextBox" runat="server" MaxLength="50" CssClass="form-control" autocomplete="username" />
                                </div>

                                <div class="mb-3">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="PasswordTextBox" Text="סיסמה" CssClass="form-label" />

                                    <div class="position-relative">
                                        <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" MaxLength="128" CssClass="form-control pe-5" ClientIDMode="Static" autocomplete="current-password" />

                                        <button type="button" id="TogglePasswordButton" class="position-absolute top-50 end-0 translate-middle-y border-0 bg-transparent text-secondary p-2 me-1" aria-label="הצג סיסמה" aria-pressed="false" title="הצג סיסמה">
                                            <i id="PasswordHiddenIcon" class="bi bi-eye fs-5" aria-hidden="true"></i>
                                            <i id="PasswordVisibleIcon" class="bi bi-eye-slash fs-5 d-none" aria-hidden="true"></i>
                                        </button>
                                    </div>
                                </div>

                                <div class="d-flex flex-column flex-sm-row justify-content-between gap-2 mb-3">
                                    <div class="form-check">
                                        <input type="checkbox" id="RememberDetailsCheckBox" runat="server" class="form-check-input" />
                                        <label for="RememberDetailsCheckBox" class="form-check-label">זכור שם משתמש</label>
                                    </div>

                                    <div class="form-check">
                                        <input type="checkbox" id="KeepSignedInCheckBox" runat="server" class="form-check-input" />
                                        <label for="KeepSignedInCheckBox" class="form-check-label">השאר מחובר</label>
                                    </div>
                                </div>

                                <asp:Button ID="LoginButton" runat="server" Text="התחברות" CssClass="btn btn-primary w-100" OnClick="LoginButton_Click" />

                                <asp:Label ID="LoginResultLabel" runat="server" CssClass="d-block text-danger text-center mt-3" />

                                <div class="text-center mt-4">
                                    <span class="text-secondary">עדיין אין לך חשבון?</span>
                                    <a href="<%: ResolveUrl("~/Register.aspx") %>" class="btn btn-link p-0 fw-semibold text-decoration-none">הירשם</a>
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </main>
        </div>
    </form>
    <script src="<%: ResolveUrl("~/Scripts/login.js?v=2") %>"></script>
    <script src="<%: ResolveUrl("~/Scripts/product-builder.js") %>"></script>
</body>
</html>
