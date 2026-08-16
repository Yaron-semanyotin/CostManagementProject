<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MeasurementUnits.aspx.cs" Inherits="CostWise.MeasurementUnits" MasterPageFile="~/Site.Master" Title="יחידות מידה" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section aria-labelledby="MeasurementUnitsHeading">
        <div class="mb-4">
            <div class="d-flex align-items-center gap-2 mb-1">
                <h2 id="MeasurementUnitsHeading" class="h4 mb-0">ניהול יחידות מידה
                </h2>

                <button type="button" class="btn btn-outline-secondary btn-sm rounded-circle d-inline-flex align-items-center justify-content-center p-0" style="width: 1.75rem; height: 1.75rem;"
                    aria-label="הסבר על משפחות יחידות המידה ומקדמי ההמרה" data-bs-toggle="tooltip" data-bs-placement="bottom"
                    title="משקל: גרם היא יחידת הבסיס וקילוגרם אחד שווה 1,000 גרם. נפח: מיליליטר היא יחידת הבסיס וליטר אחד שווה 1,000 מיליליטר. כמות: יחידה משמשת לספירה. המערכת משתמשת במקדמי ההמרה באופן אוטומטי בחישובים.">
                    ?
                </button>
            </div>

            <p class="text-secondary mb-0">
                צפייה ביחידות המערכת וניהול יחידות המידה המותאמות לעסק.
            </p>
        </div>

        <asp:Label ID="ResultLabel" runat="server" CssClass="d-block mb-3" role="status" />

        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h3 class="h5 mb-3">הוספת יחידה מותאמת</h3>

                <asp:Panel ID="AddCustomUnitPanel" runat="server" DefaultButton="AddCustomUnitButton" CssClass="row g-3 align-items-end">
                    <div class="col-12 col-md-4">
                        <asp:Label ID="CustomUnitNameLabel" runat="server" AssociatedControlID="CustomUnitNameTextBox" CssClass="form-label" Text="שם היחידה" />

                        <asp:TextBox ID="CustomUnitNameTextBox" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>

                    <div class="col-12 col-md-3">
                        <asp:Label ID="UnitFamilyLabel" runat="server" AssociatedControlID="UnitFamilyDropDownList" CssClass="form-label" Text="משפחת היחידה" />

                        <asp:DropDownList ID="UnitFamilyDropDownList" runat="server" CssClass="form-select">

                            <asp:ListItem Text="בחר משפחה" Value="" />
                            <asp:ListItem Text="משקל" Value="Weight" />
                            <asp:ListItem Text="נפח" Value="Volume" />
                            <asp:ListItem Text="כמות" Value="Quantity" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 col-md-3">
                        <asp:Label ID="ConversionFactorLabel" runat="server" AssociatedControlID="ConversionFactorTextBox" CssClass="form-label" Text="מקדם המרה ליחידת בסיס" />

                        <asp:TextBox ID="ConversionFactorTextBox" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-12 col-md-2">
                        <asp:Button ID="AddCustomUnitButton" runat="server" Text="הוסף יחידה" CssClass="btn btn-primary w-100" OnClick="AddCustomUnitButton_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>

        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h3 class="h5 mb-1">יחידות מערכת</h3>

                <p class="text-secondary mb-3">
                    יחידות משותפות הזמינות לכל העסקים ואינן ניתנות לעריכה.
                </p>

                <div class="table-responsive">
                    <asp:GridView ID="SystemUnitsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover align-middle mb-0"
                        GridLines="None" UseAccessibleHeader="true" EmptyDataText="לא נמצאו יחידות מערכת.">

                        <HeaderStyle CssClass="table-light" />

                        <Columns>
                            <asp:BoundField DataField="UnitName" HeaderText="שם היחידה" />

                            <asp:TemplateField HeaderText="משפחה">
                                <ItemTemplate>
                                    <%#: GetUnitFamilyDisplayName(Eval("UnitFamily")) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="כמות לתצוגה">
                                <ItemTemplate>
                                    <%#: GetSystemUnitDisplayQuantity(Eval("UnitName")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="card shadow-sm">
            <div class="card-body">
                <h3 class="h5 mb-1">יחידות מותאמות לעסק</h3>

                <p class="text-secondary mb-3">
                    יחידות שנוצרו עבור העסק המחובר וניתנות לעריכה או למחיקה.
                </p>

                <div class="table-responsive">
                    <asp:GridView ID="CustomUnitsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover align-middle mb-0"
                        GridLines="None" UseAccessibleHeader="true" EmptyDataText="לא הוגדרו יחידות מותאמות." DataKeyNames="MeasurementUnitId" onkeydown="return handleCustomUnitEditEnter(event);"
                        OnRowEditing="CustomUnitsGrid_RowEditing" OnRowCancelingEdit="CustomUnitsGrid_RowCancelingEdit" OnRowUpdating="CustomUnitsGrid_RowUpdating" OnRowDeleting="CustomUnitsGrid_RowDeleting">

                        <HeaderStyle CssClass="table-light" />

                        <Columns>
                            <asp:BoundField DataField="UnitName" HeaderText="שם היחידה" />

                            <asp:TemplateField HeaderText="משפחה">
                                <ItemTemplate>
                                    <%#: GetUnitFamilyDisplayName(Eval("UnitFamily")) %>
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <asp:DropDownList ID="EditUnitFamilyDropDownList" runat="server"
                                        CssClass="form-select form-select-sm" SelectedValue='<%# Bind("UnitFamily") %>'>

                                        <asp:ListItem Text="משקל" Value="Weight" />
                                        <asp:ListItem Text="נפח" Value="Volume" />
                                        <asp:ListItem Text="כמות" Value="Quantity" />
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="ConversionFactorToBase" HeaderText="מקדם המרה ליחידת בסיס" DataFormatString="{0:0.##}" HtmlEncode="false" />

                            <asp:TemplateField HeaderText="פעולות">
                                <ItemTemplate>
                                    <span class="cw-table-actions">
                                        <asp:LinkButton ID="EditUnitButton" runat="server" Text="ערוך"
                                            CommandName="Edit" CssClass="cw-action cw-action-primary" CausesValidation="false" />
                                        <asp:LinkButton ID="DeleteUnitButton" runat="server" Text="מחק"
                                            CommandName="Delete" CssClass="cw-action cw-action-danger" CausesValidation="false"
                                            OnClientClick="return confirm('האם למחוק את יחידת המידה?');" />
                                    </span>
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <span class="cw-table-actions">
                                        <asp:LinkButton ID="UpdateUnitButton" runat="server" Text="שמור"
                                            CommandName="Update" CssClass="cw-action cw-action-primary" CausesValidation="false" />
                                        <asp:LinkButton ID="CancelUnitButton" runat="server" Text="ביטול"
                                            CommandName="Cancel" CssClass="cw-action cw-action-secondary" CausesValidation="false" />
                                    </span>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </section>
    <script>
        var measurementUnitsScrollStorageKey =
            "CostWise.MeasurementUnits.ScrollPosition";

        function saveMeasurementUnitsScrollPosition() {
            try {
                window.sessionStorage.setItem(
                    measurementUnitsScrollStorageKey,
                    window.scrollY.toString()
                );
            }
            catch (error) {
                // שמירת מיקום הגלילה היא שיפור תצוגה בלבד.
            }
        }

        function restoreMeasurementUnitsScrollPosition() {
            try {
                var savedScrollPosition =
                    window.sessionStorage.getItem(
                        measurementUnitsScrollStorageKey
                    );

                if (savedScrollPosition === null) {
                    return;
                }

                window.sessionStorage.removeItem(
                    measurementUnitsScrollStorageKey
                );

                var scrollPosition =
                    parseInt(savedScrollPosition, 10);

                if (isNaN(scrollPosition)) {
                    return;
                }

                window.requestAnimationFrame(function () {
                    window.scrollTo(0, scrollPosition);
                });
            }
            catch (error) {
                // העמוד ממשיך לפעול גם אם sessionStorage אינו זמין.
            }
        }

        function handleCustomUnitEditEnter(event) {
            var targetTagName = event.target.tagName.toLowerCase();

            var isEditableControl =
                targetTagName === "input" ||
                targetTagName === "select" ||
                targetTagName === "textarea";

            if (event.key !== "Enter" || !isEditableControl) {
                return true;
            }

            event.preventDefault();

            var editedRow = event.target.closest("tr");

            if (!editedRow) {
                return false;
            }

            var actionLinks = editedRow.querySelectorAll("a");

            for (var index = 0; index < actionLinks.length; index++) {
                if (actionLinks[index].textContent.trim() === "שמור") {
                    saveMeasurementUnitsScrollPosition();
                    actionLinks[index].click();
                    break;
                }
            }

            return false;
        }

        document.addEventListener("DOMContentLoaded", function () {
            var tooltipElements =
                document.querySelectorAll('[data-bs-toggle="tooltip"]');

            tooltipElements.forEach(function (element) {
                new bootstrap.Tooltip(element);
            });

            var addCustomUnitButton =
                document.getElementById(
                '<%= AddCustomUnitButton.ClientID %>'
                );

            if (addCustomUnitButton) {
                addCustomUnitButton.addEventListener(
                    "click",
                    saveMeasurementUnitsScrollPosition
                );
            }

            var customUnitsGrid =
                document.getElementById(
                '<%= CustomUnitsGrid.ClientID %>'
                );

            if (customUnitsGrid) {
                customUnitsGrid.addEventListener("click", function (event) {
                    var actionControl =
                        event.target.closest(
                            "a, button, input[type='submit']"
                        );

                    if (actionControl) {
                        saveMeasurementUnitsScrollPosition();
                    }
                });
            }

            restoreMeasurementUnitsScrollPosition();
        });
    </script>
</asp:Content>
