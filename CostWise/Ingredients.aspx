<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Ingredients.aspx.cs" Inherits="CostWise.Ingredients" MasterPageFile="~/Site.Master" Title="רכיבים" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="IngredientsMainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section aria-labelledby="IngredientsHeading">
        <div class="mb-4">
            <h2 id="IngredientsHeading" class="h4 mb-1">ניהול רכיבים
            </h2>

            <p class="text-secondary mb-0">
                צפייה ברכיבים וניהול מחירי האריזה, הכמויות ויחידות המידה שלהם.
            </p>
        </div>

        <asp:Label ID="ResultLabel" runat="server" CssClass="d-block mb-3" role="status" />

        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h3 class="h5 mb-3">הוספת רכיב</h3>

                <asp:Panel ID="AddIngredientPanel" runat="server" DefaultButton="AddIngredientButton" CssClass="row g-3 align-items-end">

                    <div class="col-12 col-md-3">
                        <asp:Label ID="IngredientNameLabel" runat="server" AssociatedControlID="IngredientNameTextBox" CssClass="form-label" Text="שם הרכיב" />

                        <asp:TextBox ID="IngredientNameTextBox" runat="server" CssClass="form-control" MaxLength="150" />
                    </div>

                    <div class="col-12 col-md-2">
                        <asp:Label ID="PackagePriceLabel" runat="server" AssociatedControlID="PackagePriceTextBox" CssClass="form-label" Text="מחיר האריזה" />

                        <asp:TextBox ID="PackagePriceTextBox" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-12 col-md-2">
                        <asp:Label ID="PackageQuantityLabel" runat="server" AssociatedControlID="PackageQuantityTextBox" CssClass="form-label" Text="כמות באריזה" />

                        <asp:TextBox ID="PackageQuantityTextBox" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-12 col-md-3">
                        <asp:Label ID="PackageUnitLabel" runat="server" AssociatedControlID="PackageUnitDropDownList" CssClass="form-label" Text="יחידת האריזה" />

                        <asp:DropDownList ID="PackageUnitDropDownList" runat="server" CssClass="form-select">

                            <asp:ListItem
                                Text="בחר יחידת מידה" Value="" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 col-md-2">
                        <asp:Button ID="AddIngredientButton" runat="server" Text="הוסף רכיב" CssClass="btn btn-primary w-100" OnClick="AddIngredientButton_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
        <div class="card shadow-sm">
            <div class="card-body">

                <div class="d-flex align-items-center gap-2 mb-3">
                    <h3 id="ActiveIngredientsHeading" class="h5 mb-0">רשימת רכיבים
                    </h3>

                    <button type="button" class="btn btn-outline-secondary btn-sm rounded-circle d-inline-flex align-items-center justify-content-center p-0"
                        style="width: 1.75rem; height: 1.75rem;" aria-label="הסבר על ניהול רשימת הרכיבים הפעילים" data-bs-toggle="tooltip"
                        data-bs-placement="bottom" title="רשימת רכיבים פעילים. בדף זה ניתן להוסיף רכיבים, לערוך אותם או להעביר אותם לסל המחזור.">
                        ?
                    </button>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="IngredientsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover align-middle mb-0"
                        GridLines="None" UseAccessibleHeader="true" EmptyDataText="לא נמצאו רכיבים" DataKeyNames="IngredientId" onkeydown="return handleIngredientEditEnter(event);" OnRowEditing="IngredientsGrid_RowEditing"
                        OnRowCancelingEdit="IngredientsGrid_RowCancelingEdit" OnRowUpdating="IngredientsGrid_RowUpdating" OnRowDataBound="IngredientsGrid_RowDataBound" OnRowDeleting="IngredientsGrid_RowDeleting">
                        <HeaderStyle CssClass="table-light" />
                        <Columns>
                            <asp:BoundField DataField="IngredientName" HeaderText="שם הרכיב">

                                <ControlStyle CssClass="form-control form-control-sm" />
                            </asp:BoundField>

                            <asp:BoundField DataField="PackagePrice" HeaderText="מחיר האריזה" DataFormatString="{0:0.00}" HtmlEncode="false">

                                <ControlStyle CssClass="form-control form-control-sm" />
                            </asp:BoundField>

                            <asp:BoundField DataField="PackageQuantity" HeaderText="כמות באריזה" DataFormatString="{0:0.######}" HtmlEncode="false">

                                <ControlStyle CssClass="form-control form-control-sm" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="יחידת האריזה">
                                <ItemTemplate>
                                    <asp:Label ID="PackageUnitNameLabel" runat="server" Text='<%# Eval("PackageUnitName") %>' />
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <asp:DropDownList ID="EditPackageUnitDropDownList" runat="server" CssClass="form-select form-select-sm" />
                                </EditItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="פעולות">
                                <ItemTemplate>
                                    <span class="cw-table-actions">
                                        <asp:LinkButton ID="EditIngredientButton" runat="server" Text="ערוך"
                                            CommandName="Edit" CssClass="cw-action cw-action-primary" CausesValidation="false" />
                                        <asp:LinkButton ID="DeleteIngredientButton" runat="server" Text="מחק"
                                            CommandName="Delete" CssClass="cw-action cw-action-danger" CausesValidation="false"
                                            OnClientClick="return confirm('האם להעביר את הרכיב לסל המחזור?');" />
                                    </span>
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <span class="cw-table-actions">
                                        <asp:LinkButton ID="UpdateIngredientButton" runat="server" Text="שמור"
                                            CommandName="Update" CssClass="cw-action cw-action-primary" CausesValidation="false" />
                                        <asp:LinkButton ID="CancelIngredientButton" runat="server" Text="ביטול"
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
        var ingredientsScrollStorageKey =
            "CostWise.Ingredients.ScrollPosition";

        function saveIngredientsScrollPosition() {
            try {
                window.sessionStorage.setItem(
                    ingredientsScrollStorageKey,
                    window.scrollY.toString()
                );
            }
            catch (error) {
                // שמירת מיקום הגלילה היא שיפור תצוגה בלבד.
            }
        }

        function restoreIngredientsScrollPosition() {
            try {
                var savedScrollPosition =
                    window.sessionStorage.getItem(
                        ingredientsScrollStorageKey
                    );

                if (savedScrollPosition === null) {
                    return;
                }

                window.sessionStorage.removeItem(
                    ingredientsScrollStorageKey
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

        function handleIngredientEditEnter(event) {
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
                    saveIngredientsScrollPosition();
                    actionLinks[index].click();
                    break;
                }
            }

            return false;
        }

        document.addEventListener("DOMContentLoaded", function () {
            var tooltipElements =
                document.querySelectorAll(
                    '[data-bs-toggle="tooltip"]'
                );

            tooltipElements.forEach(function (element) {
                new bootstrap.Tooltip(element);
            });
            var addIngredientButton =
                document.getElementById(
                '<%= AddIngredientButton.ClientID %>'
                );

            if (addIngredientButton) {
                addIngredientButton.addEventListener(
                    "click",
                    saveIngredientsScrollPosition
                );
            }

            var ingredientsGrid =
                document.getElementById(
                '<%= IngredientsGrid.ClientID %>'
                );

            if (ingredientsGrid) {
                ingredientsGrid.addEventListener("click", function (event) {
                    var actionControl =
                        event.target.closest(
                            "a, button, input[type='submit']"
                        );

                    if (actionControl) {
                        saveIngredientsScrollPosition();
                    }
                });
            }

            restoreIngredientsScrollPosition();
        });
    </script>
</asp:Content>
