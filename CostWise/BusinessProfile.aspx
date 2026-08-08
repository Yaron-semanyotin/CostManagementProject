<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BusinessProfile.aspx.cs" Inherits="CostWise.BusinessProfile" MasterPageFile="~/Site.Master" Title="פרופיל העסק" %>

<asp:Content ID="BusinessProfileMainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section aria-labelledby="BusinessProfileHeading">
        <div class="card shadow-sm">
            <div class="card-body">
                <div class="d-flex align-items-center gap-3">
                    <asp:Image ID="BusinessLogoImage" runat="server" Width="64" Height="64" CssClass="rounded-circle object-fit-cover border" AlternateText="לוגו העסק" Visible="false" />

                    <asp:Panel ID="BusinessLogoFallbackPanel" runat="server" CssClass="d-inline-flex align-items-center justify-content-center rounded-circle bg-primary text-white fw-bold"
                        Style="width: 64px; height: 64px;" role="img" aria-label="לוגו ברירת מחדל של CostWise">
                        CW
                    </asp:Panel>

                    <div>
                        <h2 id="BusinessProfileHeading" class="h4 mb-1">פרטי העסק </h2>

                        <p class="text-secondary mb-0">
                            צפייה ועדכון של פרטי העסק המחובר.
                        </p>
                    </div>
                </div>

                <hr class="my-4" />

                <asp:Panel ID="BusinessProfileFormPanel" runat="server" DefaultButton="SaveBusinessButton">

                    <div class="row">
                        <div class="col-12 col-lg-6">
                            <asp:Label ID="BusinessNameFieldLabel" runat="server" AssociatedControlID="BusinessNameTextBox" CssClass="form-label" Text="שם העסק" />

                            <asp:TextBox ID="BusinessNameTextBox" runat="server" CssClass="form-control" MaxLength="150" />

                            <asp:RequiredFieldValidator ID="BusinessNameRequiredValidator" runat="server" ControlToValidate="BusinessNameTextBox"
                                ValidationGroup="BusinessProfile" ErrorMessage="שם העסק הוא שדה חובה." CssClass="text-danger" Display="Dynamic" />
                        </div>
                    </div>

                    <div class="mt-4">
                        <asp:Button ID="SaveBusinessButton" runat="server" Text="שמור שינויים" CssClass="btn btn-primary" ValidationGroup="BusinessProfile" OnClick="SaveBusinessButton_Click" />
                    </div>

                    <asp:Label ID="ResultLabel" runat="server" CssClass="d-block mt-3" role="status" />

                </asp:Panel>
                <hr class="my-4" />

                <section aria-labelledby="BusinessLogoHeading">
                    <h3 id="BusinessLogoHeading" class="h5">לוגו העסק </h3>

                    <p class="text-secondary">
                        ניתן להעלות קובץ JPG או PNG בגודל של עד 2MB.
                    </p>

                    <div class="row">
                        <div class="col-12 col-lg-6">
                            <asp:Label ID="BusinessLogoFileLabel" runat="server" AssociatedControlID="BusinessLogoFileUpload" CssClass="form-label" Text="בחירת קובץ לוגו" />

                            <asp:FileUpload ID="BusinessLogoFileUpload" runat="server" CssClass="form-control" accept=".jpg,.jpeg,.png,image/jpeg,image/png" />

                            <div class="form-text">
                                סוגים מותרים: JPG ו־PNG. גודל מרבי: 2MB.
                            </div>
                        </div>
                    </div>

                    <div class="mt-3">
                        <asp:Button ID="UploadLogoButton" runat="server" Text="העלה לוגו" CssClass="btn btn-outline-primary" CausesValidation="false" OnClick="UploadLogoButton_Click" />
                    </div>

                    <asp:Label ID="LogoResultLabel" runat="server" CssClass="d-block mt-3" role="status" />
                </section>
            </div>
        </div>
    </section>

</asp:Content>
