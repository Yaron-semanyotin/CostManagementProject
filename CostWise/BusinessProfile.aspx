<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BusinessProfile.aspx.cs" Inherits="CostWise.BusinessProfile" MasterPageFile="~/Site.Master" Title="פרופיל העסק" %>

<asp:Content ID="BusinessProfileMainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section aria-labelledby="BusinessProfileHeading">
        <div class="card shadow-sm">
            <div class="card-body">
                <div class="d-flex align-items-center gap-3">
                    <asp:Image ID="BusinessLogoImage" runat="server" Width="64" Height="64" CssClass="rounded-circle object-fit-cover border" AlternateText="לוגו העסק" Visible="false" />

                    <asp:Panel ID="BusinessLogoFallbackPanel" runat="server" CssClass="d-inline-flex align-items-center justify-content-center rounded-circle bg-primary text-white fw-bold"
                        Style="width: 64px; height: 64px;" role="img" aria-label="לוגו ברירת מחדל של CoTamhirostWise">
                        T
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
                <section aria-labelledby="BusinessSettingsHeading">
                    <h3 id="BusinessSettingsHeading" class="h5">הגדרות מוצרים וחישובים
    </h3>

                    <p class="text-secondary">
                        הגדר כיצד יוצגו יחידות המידה ומהו שיעור המע״מ של העסק.
   
                    </p>

                    <asp:Panel ID="BusinessSettingsPanel" runat="server">

                        <div class="row g-4">
                            <div class="col-12">
                                <div class="form-check form-switch">
                                    <input id="ShowYieldUnitSelectionSwitch" runat="server" clientidmode="Static"
                                        type="checkbox" class="form-check-input" role="switch" />

                                    <label class="form-check-label" for="ShowYieldUnitSelectionSwitch">
                                        הצג בחירת יחידת תוצר בעת יצירה ועריכת מוצר
                   
                                    </label>
                                </div>

                                <div class="form-text">
                                    כאשר האפשרות כבויה, המערכת תשתמש אוטומטית ביחידה.
               
                                </div>
                            </div>

                            <div class="col-12 col-lg-6">
                                <asp:Label ID="DefaultRecipeMeasurementUnitLabel" runat="server"
                                    AssociatedControlID="DefaultRecipeMeasurementUnitDropDownList" CssClass="form-label"
                                    Text="יחידת ברירת מחדל לרכיבי המתכון" />

                                <asp:DropDownList ID="DefaultRecipeMeasurementUnitDropDownList" runat="server" CssClass="form-select">

                                    <asp:ListItem Text="ללא יחידת ברירת מחדל" Value="" />
                                </asp:DropDownList>

                                <div class="form-text">
                                    היחידה תיבחר אוטומטית בכל שורת מתכון חדשה.
               
                                </div>
                            </div>

                            <div class="col-12 col-lg-6">
                                <asp:Label ID="VatRatePercentLabel" runat="server" AssociatedControlID="VatRatePercentTextBox"
                                    CssClass="form-label" Text="שיעור מע״מ באחוזים" />

                                <asp:TextBox ID="VatRatePercentTextBox" runat="server" CssClass="form-control" MaxLength="6" inputmode="decimal" />

                                <asp:RequiredFieldValidator ID="VatRatePercentRequiredValidator" runat="server" ControlToValidate="VatRatePercentTextBox"
                                    ValidationGroup="BusinessProfile" ErrorMessage="שיעור המע״מ הוא שדה חובה." CssClass="text-danger" Display="Dynamic" />
                            </div>
                        </div>

                    </asp:Panel>
                </section>

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

                </section>
            </div>
        </div>
    </section>

</asp:Content>
