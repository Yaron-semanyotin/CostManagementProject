<%@ Page
    Language="C#"
    AutoEventWireup="true"
    CodeFile="ProductRecycleBin.aspx.cs"
    Inherits="CostWise.ProductRecycleBin"
    MasterPageFile="~/Site.Master"
    Title="סל מחזור מוצרים"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="ProductRecycleBinMainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <section
        aria-labelledby="ProductRecycleBinHeading">

        <div class="d-flex flex-wrap align-items-center justify-content-between gap-3 mb-4">
            <div>
                <h2
                    id="ProductRecycleBinHeading"
                    class="h4 mb-1">סל מחזור מוצרים
                </h2>

                <p class="text-secondary mb-0">
                    כאן ניתן לצפות במוצרים מושבתים ולשחזר אותם.
                </p>
            </div>

            <asp:HyperLink
                ID="BackToProductsLink"
                runat="server"
                NavigateUrl="~/Products.aspx"
                CssClass="btn btn-outline-secondary"
                Text="חזרה למוצרים" />
        </div>

        <asp:Label
            ID="ResultLabel"
            runat="server"
            CssClass="d-block mb-3"
            role="status" />
        <div class="card shadow-sm">
            <div class="card-body">
                <h3 class="h5 mb-3">מוצרים בסל המחזור
                </h3>

                <div class="table-responsive">
                    <asp:GridView
                        ID="ProductRecycleBinGrid"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-striped table-hover align-middle mb-0"
                        GridLines="None"
                        UseAccessibleHeader="true"
                        EmptyDataText="סל המחזור ריק."
                        DataKeyNames="ProductId"
                        OnRowCommand="ProductRecycleBinGrid_RowCommand">

                        <headerstyle cssclass="table-light" />

                        <columns>
                            <asp:BoundField
                                DataField="ProductName"
                                HeaderText="שם המוצר" />

                            <asp:BoundField
                                DataField="YieldQuantity"
                                HeaderText="כמות תוצר"
                                DataFormatString="{0:0.######}"
                                HtmlEncode="false" />

                            <asp:BoundField
                                DataField="YieldUnitLabel"
                                HeaderText="יחידת תוצר" />

                            <asp:ButtonField
                                Text="שחזר מוצר"
                                CommandName="RestoreProduct"
                                HeaderText="פעולות" />
                        </columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
