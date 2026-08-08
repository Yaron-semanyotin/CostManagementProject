<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="CostWise.Products" MasterPageFile="~/Site.Master" Title="מוצרים" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="ProductsMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <section aria-labelledby="ProductsHeading">
        <div class="mb-4">
            <h2 id="ProductsHeading" class="h4 mb-1">ניהול מוצרים
            </h2>

            <p class="text-secondary mb-0">
                יצירת מוצרים, הגדרת כמות התוצר והמשך ישיר לבניית המתכון.
            </p>
        </div>

        <asp:Label ID="ResultLabel" runat="server" CssClass="d-block mb-3" role="status" />

        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h3 class="h5 mb-3">הוספת מוצר</h3>

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

                        <asp:DropDownList ID="YieldUnitDropDownList" runat="server" CssClass="form-select">
                            <asp:ListItem Text="בחר יחידת מידה" Value="" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 col-md-6 col-xl-2">
                        <asp:Button ID="AddProductButton" runat="server" Text="צור מוצר והמשך למתכון" CssClass="btn btn-primary w-100" OnClick="AddProductButton_Click" />
                    </div>
                </div>
            </div>
        </div>
        <div class="card shadow-sm">
            <div class="card-body">
                <div class="d-flex align-items-center gap-2 mb-3">
                    <h3 id="ProductsListHeading" class="h5 mb-0">רשימת מוצרים
                    </h3>

                    <button type="button" class="btn btn-outline-secondary btn-sm rounded-circle d-inline-flex align-items-center justify-content-center p-0" style="width: 1.75rem; height: 1.75rem;"
                        aria-label="הסבר על ניהול רשימת המוצרים" data-bs-toggle="tooltip" data-bs-placement="bottom" title="ברשימה זו ניתן לערוך מוצרים, להשבית אותם ולנהל את פרטי התוצר שלהם.">
                        ?
                    </button>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="ProductsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover align-middle mb-0"
                        GridLines="None" UseAccessibleHeader="true" EmptyDataText="לא נמצאו מוצרים" DataKeyNames="ProductId,IsActive"
                        OnRowEditing="ProductsGrid_RowEditing" OnRowCancelingEdit="ProductsGrid_RowCancelingEdit" OnRowUpdating="ProductsGrid_RowUpdating" OnRowDeleting="ProductsGrid_RowDeleting" OnRowDataBound="ProductsGrid_RowDataBound">

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

                                <EditItemTemplate>
                                    <asp:DropDownList ID="EditYieldUnitDropDownList" runat="server" CssClass="form-select form-select-sm" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="מתכון">
                                <ItemTemplate>
                                    <asp:HyperLink ID="RecipeLink" runat="server" Text="פתח מתכון"
                                        CssClass="btn btn-outline-primary btn-sm" NavigateUrl='<%# Eval("ProductId", "~/Recipe.aspx?productId={0}") %>' Visible='<%# (bool)Eval("IsActive") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="IsActive" HeaderText="פעיל" ReadOnly="true" />

                            <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="ערוך" UpdateText="שמור" CancelText="ביטול" DeleteText="השבת" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
