<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IngredientRecycleBin.aspx.cs" Inherits="CostWise.IngredientRecycleBin" MasterPageFile="~/Site.Master" Title="סל מחזור רכיבים" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="IngredientRecycleBinMainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section aria-labelledby="IngredientRecycleBinHeading">
        <div class="mb-4">
            <h2
                id="IngredientRecycleBinHeading"
                class="h4 mb-1">סל מחזור רכיבים
            </h2>

            <p class="text-secondary mb-0">
                צפייה ברכיבים שהועברו לסל המחזור ושחזורם לרשימת הרכיבים.
            </p>
        </div>
        <asp:Label ID="ResultLabel" runat="server" CssClass="d-block mb-3" role="status" />

        <div class="card shadow-sm">
            <div class="card-body">
                <h3 class="h5 mb-3">רכיבים בסל המחזור
                </h3>

                <div class="table-responsive">
                    <asp:GridView ID="RecycleBinGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover align-middle mb-0"
                        GridLines="None" UseAccessibleHeader="true" EmptyDataText="סל המחזור ריק." DataKeyNames="IngredientId" OnRowCommand="RecycleBinGrid_RowCommand">

                        <HeaderStyle CssClass="table-light" />

                        <Columns>
                            <asp:BoundField DataField="IngredientName" HeaderText="שם הרכיב" />

                            <asp:BoundField DataField="PackagePrice" HeaderText="מחיר האריזה" DataFormatString="{0:0.00}" HtmlEncode="false" />

                            <asp:BoundField DataField="PackageQuantity" HeaderText="כמות באריזה" DataFormatString="{0:0.######}" HtmlEncode="false" />

                            <asp:BoundField DataField="PackageUnitName" HeaderText="יחידת האריזה" />

                            <asp:ButtonField Text="שחזור רכיב" CommandName="RestoreIngredient" HeaderText="פעולות">
                                <ControlStyle CssClass="cw-action cw-action-primary" />
                            </asp:ButtonField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
