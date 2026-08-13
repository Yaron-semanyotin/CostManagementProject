<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="CostWise.Products" MasterPageFile="~/Site.Master" Title="מוצרים" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="ProductsMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <section id="ProductBuilderPage" aria-labelledby="ProductsHeading"
        data-builder-endpoint="<%: ResolveUrl("~/api/product-builder-data") %>" data-cost-preview-endpoint="<%: ResolveUrl("~/api/product-builder-data/ingredient-cost-preview") %>"
        data-login-url="<%: ResolveUrl("~/Login.aspx") %>" data-result-label-id="<%: ResultLabel.ClientID %>" data-clear-builder-cache="<%= ShouldClearProductBuilderDataCache ? "true" : "false" %>"
        data-recipe-json-field-id="RecipeIngredientsJsonHiddenField" data-editing-product-id="<%: EditingProductId %>">

        <div class="mb-4">
            <h2 id="ProductsHeading" class="h4 mb-1">ניהול מוצרים
            </h2>

            <p class="text-secondary mb-0">
                יצירת מוצרים, הגדרת כמות התוצר והמשך ישיר לבניית המתכון.
            </p>
        </div>

        <asp:Label ID="ResultLabel" runat="server" CssClass="d-block mb-3" role="status" />
        <asp:HiddenField ID="RecipeIngredientsJsonHiddenField" runat="server" ClientIDMode="Static" />
        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h3 class="h5 mb-3">
                    <asp:Literal ID="ProductFormTitleLiteral" runat="server" Text="הוספת מוצר" />

                </h3>

                <div class="row g-3 align-items-end">
                    <div class="col-12 col-md-6 col-xl-4">
                        <asp:Label ID="ProductNameLabel" runat="server" AssociatedControlID="ProductNameTextBox" CssClass="form-label" Text="שם המוצר" />

                        <asp:TextBox ID="ProductNameTextBox" runat="server" CssClass="form-control" MaxLength="150" />
                    </div>

                    <div class="col-12 col-md-6 col-xl-3">
                        <asp:Label ID="YieldQuantityLabel" runat="server" AssociatedControlID="YieldQuantityTextBox" CssClass="form-label" Text="כמות תוצר" />

                        <asp:TextBox ID="YieldQuantityTextBox" runat="server" CssClass="form-control" inputmode="decimal" />
                    </div>

                    <div class="col-12 col-md-6 col-xl-3">
                        <asp:Label ID="YieldUnitLabel" runat="server" AssociatedControlID="YieldUnitDropDownList" CssClass="form-label" Text="יחידת תוצר" />

                        <asp:DropDownList ID="YieldUnitDropDownList"
                            runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="בחר יחידת מידה" Value="" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 col-md-6 col-xl-2">
                        <div class="d-grid gap-2">
                            <asp:Button ID="AddProductButton" runat="server" ClientIDMode="Static"
                                Text="צור מוצר" CssClass="btn btn-primary" OnClick="AddProductButton_Click" />

                            <asp:Button ID="CancelProductEditButton" runat="server" Text="ביטול עריכה"
                                CssClass="btn btn-outline-secondary" CausesValidation="false" Visible="false" OnClick="CancelProductEditButton_Click" />
                        </div>
                    </div>
                </div>

                <div class="border-top mt-4 pt-4"
                    aria-labelledby="RecipeIngredientsHeading">

                    <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-2">
                        <h4 id="RecipeIngredientsHeading" class="h6 mb-0">רכיבי המתכון
                        </h4>

                        <button id="AddRecipeIngredientButton" type="button" class="btn btn-outline-primary btn-sm" aria-controls="RecipeIngredientRows" disabled>
                            <span aria-hidden="true">+</span>
                            הוסף רכיב
                        </button>
                    </div>

                    <p class="text-secondary small mb-3">
                        בחר רכיב קיים והזן את הכמות הנדרשת למתכון.
                    </p>

                    <div id="RecipeIngredientRows" class="vstack gap-3" aria-live="polite" aria-busy="true">
                    </div>
                    <template id="RecipeIngredientRowTemplate">
                        <div class="recipe-ingredient-row border rounded-3 bg-light p-3">
                            <div class="row g-3 align-items-start">

                                <div class="col-12 col-lg-4 position-relative">
                                    <label class="form-label" data-role="ingredient-label">רכיב</label>

                                    <input type="text" class="form-control" data-role="ingredient-search" autocomplete="off" role="combobox" aria-autocomplete="list" aria-expanded="false" />

                                    <input type="hidden" data-role="ingredient-id" />

                                    <div class="list-group position-absolute start-0 end-0 shadow-sm" data-role="ingredient-suggestions"
                                        role="listbox" style="z-index: 1050; max-height: 16rem; overflow-y: auto;" hidden>
                                    </div>

                                    <div
                                        class="invalid-feedback"
                                        data-role="ingredient-error">
                                    </div>
                                </div>

                                <div class="col-12 col-md-3 col-lg-2">
                                    <label class="form-label" data-role="unit-label">יחידת מידה</label>

                                    <select class="form-select" data-role="recipe-unit" disabled>
                                        <option value="">בחר יחידה</option>
                                    </select>

                                    <div
                                        class="invalid-feedback"
                                        data-role="unit-error">
                                    </div>
                                </div>

                                <div class="col-12 col-md-3 col-lg-2">
                                    <label class="form-label" data-role="quantity-label">
                                        כמות במתכון
                                    </label>

                                    <input type="text" class="form-control" data-role="recipe-quantity" inputmode="decimal" />

                                    <div
                                        class="invalid-feedback"
                                        data-role="quantity-error">
                                    </div>
                                </div>

                                <div class="col-12 col-md-6 col-lg-3">
                                    <label class="form-label" data-role="ingredient-cost-label">מחיר לכמות (₪)</label>

                                    <input type="text" class="form-control" data-role="ingredient-cost" inputmode="decimal" data-manually-edited="false" value="" disabled />

                                    <div class="form-text">
                                        המחיר יחושב לאחר בחירת רכיב, כמות ויחידה.
                                    </div>

                                    <div
                                        class="invalid-feedback"
                                        data-role="ingredient-cost-error">
                                    </div>
                                </div>

                                <div class="col-12 col-lg-1 d-flex align-items-end">
                                    <button type="button" class="btn btn-outline-danger w-100" data-role="remove-row" disabled>
                                        הסר
                                    </button>
                                </div>
                            </div>
                        </div>
                    </template>
                </div>
            </div>
        </div>

        <div class="card shadow-sm">
            <div class="card-body">
                <div class="d-flex align-items-center gap-2 mb-3">
                    <h3 id="ProductsListHeading" class="h5 mb-0">רשימת מוצרים
                    </h3>

                    <button type="button" class="btn btn-outline-secondary btn-sm rounded-circle d-inline-flex align-items-center justify-content-center p-0"
                        style="width: 1.75rem; height: 1.75rem;" aria-label="הסבר על ניהול רשימת המוצרים" data-bs-toggle="tooltip" data-bs-placement="bottom" title="ברשימה זו ניתן לערוך מוצרים, להשבית אותם ולנהל את פרטי התוצר שלהם.">
                        ?
                    </button>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="ProductsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover align-middle mb-0"
                        GridLines="None" UseAccessibleHeader="true" EmptyDataText="לא נמצאו מוצרים" DataKeyNames="ProductId"
                        OnRowEditing="ProductsGrid_RowEditing" OnRowDeleting="ProductsGrid_RowDeleting" OnRowCommand="ProductsGrid_RowCommand">

                        <HeaderStyle CssClass="table-light" />

                        <Columns>
                            <asp:BoundField DataField="ProductName" HeaderText="שם המוצר">
                                <ControlStyle CssClass="form-control form-control-sm" />
                            </asp:BoundField>

                            <asp:BoundField DataField="YieldQuantity" HeaderText="כמות תוצר" DataFormatString="{0:0.######}" HtmlEncode="false">
                                <ControlStyle CssClass="form-control form-control-sm" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="יחידת תוצר">
                                <ItemTemplate>
                                    <asp:Label ID="YieldUnitNameLabel" runat="server" Text='<%# Eval("YieldUnitLabel") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="מחיר סופי">
                                <ItemTemplate>
                                    <asp:Label ID="CurrentTotalCostLabel" runat="server"
                                        Text='<%# Eval("CurrentTotalCost") == null? "לא זמין": string.Format("{0:N2} ₪", Eval("CurrentTotalCost")) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="מתכון">
                                <ItemTemplate>
                                    <asp:LinkButton ID="OpenRecipeButton" runat="server" Text="פתח מתכון" CssClass="btn btn-outline-primary btn-sm"
                                        CommandName="OpenRecipe" CommandArgument='<%# Eval("ProductId") %>' CausesValidation="false" data-role="open-recipe"
                                        data-product-id='<%# Eval("ProductId") %>' aria-expanded="false" aria-controls="ProductRecipeDetailsPanel" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField HeaderText="פעולות" ShowEditButton="true" ShowDeleteButton="true"
                                EditText="ערוך" DeleteText="מחק" />
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:Panel ID="ProductRecipeDetailsPanel" runat="server" ClientIDMode="Static" Visible="false" CssClass="border rounded-3 bg-light mt-4 p-3" role="region" aria-labelledby="RecipeDetailsHeading">

                    <h4
                        id="RecipeDetailsHeading"
                        class="h5 mb-3">פרטי המתכון:
                        <asp:Label ID="RecipeProductNameLabel" runat="server" />
                    </h4>

                    <div class="row g-3 mb-3">
                        <div class="col-12 col-sm-6 col-xl-3">
                            <div class="bg-white border rounded p-3 h-100">
                                <span class="d-block text-secondary small">כמות תוצר
                                </span>

                                <asp:Label ID="RecipeYieldQuantityLabel" runat="server" CssClass="fw-semibold" />
                            </div>
                        </div>

                        <div class="col-12 col-sm-6 col-xl-3">
                            <div class="bg-white border rounded p-3 h-100">
                                <span class="d-block text-secondary small">יחידת תוצר
                                </span>

                                <asp:Label ID="RecipeYieldUnitLabel" runat="server" CssClass="fw-semibold" />
                            </div>
                        </div>

                        <div class="col-12 col-sm-6 col-xl-3">
                            <div class="bg-white border rounded p-3 h-100">
                                <span class="d-block text-secondary small">עלות כוללת
                                </span>

                                <asp:Label ID="RecipeTotalCostLabel" runat="server" CssClass="fw-semibold" />
                            </div>
                        </div>

                        <div class="col-12 col-sm-6 col-xl-3">
                            <div class="bg-white border rounded p-3 h-100">
                                <span class="d-block text-secondary small">עלות ליחידת תוצר
                                </span>

                                <asp:Label ID="RecipeCostPerYieldUnitLabel" runat="server" CssClass="fw-semibold" />
                            </div>
                        </div>
                    </div>
                    <h5 class="h6 mb-2">רכיבי המתכון
                    </h5>

                    <div class="table-responsive">
                        <asp:GridView ID="ProductRecipeItemsGrid" runat="server" AutoGenerateColumns="false"
                            CssClass="table table-striped table-hover align-middle mb-0" GridLines="None" UseAccessibleHeader="true"
                            EmptyDataText="לא נמצאו רכיבים במתכון.">

                            <HeaderStyle CssClass="table-light" />

                            <Columns>
                                <asp:BoundField DataField="IngredientNameSnapshot" HeaderText="רכיב" />

                                <asp:BoundField DataField="RecipeQuantitySnapshot" HeaderText="כמות במתכון" DataFormatString="{0:0.######}" HtmlEncode="false" />

                                <asp:BoundField DataField="RecipeUnitNameSnapshot" HeaderText="יחידת מידה" />

                                <asp:BoundField DataField="PackagePriceSnapshot" HeaderText="מחיר האריזה הנוכחי" DataFormatString="{0:N2} ₪" HtmlEncode="false" />

                                <asp:BoundField DataField="IngredientCostSnapshot" HeaderText="מחיר לכמות" DataFormatString="{0:N2} ₪" HtmlEncode="false" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </section>
    <script src="<%: ResolveUrl("~/Scripts/product-builder.js") %>?v=3"></script>
    <script src="<%: ResolveUrl("~/Scripts/product-recipe-details.js") %>"></script>
</asp:Content>
