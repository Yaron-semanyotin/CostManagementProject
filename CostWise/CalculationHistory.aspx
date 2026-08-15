<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalculationHistory.aspx.cs"
    Inherits="CostWise.CalculationHistory" MasterPageFile="~/Site.Master" Title="היסטוריית חישובים" %>

<asp:Content ID="CalculationHistoryMainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>היסטוריית חישובים</h2>

        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h3 class="h6 mb-3">סינון לפי טווח תאריכים
                </h3>

                <div class="row g-3 align-items-end">
                    <div class="col-12 col-sm-auto">
                        <asp:Label ID="DateFromLabel" runat="server" AssociatedControlID="DateFromTextBox"
                            Text="מתאריך" CssClass="form-label" />

                        <asp:TextBox ID="DateFromTextBox" runat="server" TextMode="Date" CssClass="form-control" Style="width: 11rem; max-width: 100%;" />
                    </div>

                    <div class="col-12 col-sm-auto">
                        <asp:Label ID="DateToLabel" runat="server" AssociatedControlID="DateToTextBox"
                            Text="עד תאריך" CssClass="form-label" />

                        <asp:TextBox ID="DateToTextBox" runat="server" TextMode="Date" CssClass="form-control" Style="width: 11rem; max-width: 100%;" />
                    </div>

                    <div class="col-12 col-sm-auto">
                        <div class="d-flex gap-2">
                            <asp:Button ID="ApplyDateFilterButton" runat="server" Text="סנן" CssClass="btn btn-primary" CausesValidation="false" OnClick="ApplyDateFilterButton_Click" />

                            <asp:Button ID="ClearDateFilterButton" runat="server"
                                Text="נקה סינון" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="ClearDateFilterButton_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <asp:Label ID="ResultLabel" runat="server" />

        <asp:Repeater
            ID="ProductHistoryRepeater"
            runat="server">

            <itemtemplate>
                <div class="card shadow-sm mb-3">
                    <div class="card-header p-0">
                        <button
                            type="button"
                            class="btn w-100 text-start d-flex justify-content-between align-items-center p-3"
                            data-bs-toggle="collapse"
                            data-bs-target='<%# "#productHistory" + Eval("ProductId") %>'
                            aria-expanded='<%# GetProductHistoryAriaExpanded(Eval("ProductId")) %>'
                            aria-controls='<%# "productHistory" + Eval("ProductId") %>'>

                            <span class="fw-semibold">
                                <%#: Eval("ProductName") %>
                            </span>

                            <span aria-hidden="true">▾</span>
                        </button>
                    </div>

                    <div id='<%# "productHistory" + Eval("ProductId") %>' class='<%# GetProductHistoryCollapseCssClass(Eval("ProductId")) %>'>
                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:Repeater
                                    ID="CalculationRowsRepeater"
                                    runat="server"
                                    DataSource='<%# Eval("Calculations") %>'>

                                    <headertemplate>
                                        <table class="table table-striped align-middle mb-0">
                                            <thead>
                                                <tr>
                                                    <th scope="col">שם המוצר</th>
                                                    <th scope="col">עלות כוללת</th>
                                                    <th scope="col">עלות ליחידת תוצר</th>
                                                    <th scope="col">תקופת העלות</th>
                                                    <th scope="col">פרטים</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </headertemplate>

                                    <itemtemplate>
                                        <tr>
                                            <td><%#: Eval("ProductNameSnapshot") %></td>
                                            <td><%#: FormatCost(Eval("TotalIngredientCostSnapshot")) %></td>
                                            <td><%#: FormatCost(Eval("CostPerYieldUnitSnapshot")) %></td>
                                            <td><%#: FormatHistoryPeriod(Eval("CalculatedAtUtc"),Eval("ValidUntilUtc")) %></td>
                                            <td>
                                                <asp:LinkButton ID="OpenCalculationDetailsButton" runat="server"
                                                    Text="הצג פרטים" CssClass="btn btn-sm btn-outline-primary" CommandArgument='<%# Eval("CostCalculationId") %>'
                                                    CausesValidation="false" OnCommand="OpenCalculationDetailsButton_Command" />
                                            </td>
                                        </tr>
                                        <asp:Repeater ID="InlineCalculationDetailsRepeater" runat="server"
                                            DataSource='<%# GetInlineCalculationDetails(Eval("CostCalculationId")) %>'>

                                            <itemtemplate>
                                                <tr>
                                                    <td colspan="5">
                                                        <div class="border rounded-3 bg-light p-3 my-2">
                                                            <h4 class="h6 mb-3">פרטי חישוב היסטורי:<%#: Eval("Calculation.ProductNameSnapshot") %>
                                                            </h4>

                                                            <div class="row g-3 mb-3">
                                                                <div class="col-12 col-sm-6 col-xl-3">
                                                                    <div class="bg-white border rounded p-3 h-100">
                                                                        <span class="d-block text-secondary small">תפוקת המוצר
                                                                        </span>

                                                                        <span class="fw-semibold">
                                                                            <%#: string.Format("{0:0.######} {1}",Eval("Calculation.YieldQuantitySnapshot"),Eval("Calculation.YieldUnitLabelSnapshot")) %>
                                                                        </span>
                                                                    </div>
                                                                </div>

                                                                <div class="col-12 col-sm-6 col-xl-3">
                                                                    <div class="bg-white border rounded p-3 h-100">
                                                                        <span class="d-block text-secondary small">עלות כוללת
                                                                        </span>

                                                                        <span class="fw-semibold">
                                                                            <%#: FormatCost(Eval("Calculation.TotalIngredientCostSnapshot")) %>
                                                                        </span>
                                                                    </div>
                                                                </div>

                                                                <div class="col-12 col-sm-6 col-xl-3">
                                                                    <div class="bg-white border rounded p-3 h-100">
                                                                        <span class="d-block text-secondary small">עלות ליחידת תוצר
                                                                        </span>

                                                                        <span class="fw-semibold">
                                                                            <%#: FormatCost(Eval("Calculation.CostPerYieldUnitSnapshot")) %>
                                                                        </span>
                                                                    </div>
                                                                </div>

                                                                <div class="col-12 col-sm-6 col-xl-3">
                                                                    <div class="bg-white border rounded p-3 h-100">
                                                                        <span class="d-block text-secondary small">מועד החישוב
                                                                        </span>

                                                                        <span class="fw-semibold">
                                                                            <%#: Convert.ToDateTime(Eval("Calculation.CalculatedAtUtc")).ToString("בתאריך: dd/MM/yyyy בשעה: HH:mm") %>
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="table-responsive">
                                                                <asp:GridView ID="InlineCalculationItemsGrid" runat="server" DataSource='<%# Eval("Items") %>'
                                                                    AutoGenerateColumns="false" CssClass="table table-striped align-middle mb-0" GridLines="None"
                                                                    UseAccessibleHeader="true" EmptyDataText="לא נמצאו רכיבים בחישוב">

                                                                    <headerstyle cssclass="table-light" />

                                                                    <columns>
                                                                        <asp:BoundField DataField="IngredientNameSnapshot" HeaderText="רכיב" />
                                                                        <asp:BoundField DataField="RecipeQuantitySnapshot" HeaderText="כמות במתכון" DataFormatString="{0:0.######}" HtmlEncode="false" />
                                                                        <asp:BoundField DataField="RecipeUnitNameSnapshot" HeaderText="יחידת מידה" />
                                                                        <asp:BoundField DataField="PackagePriceSnapshot" HeaderText="מחיר אריזה" DataFormatString="{0:N2} ₪" HtmlEncode="false" />
                                                                        <asp:BoundField DataField="PackageQuantitySnapshot" HeaderText="כמות באריזה" DataFormatString="{0:0.######}" HtmlEncode="false" />
                                                                        <asp:BoundField DataField="PackageUnitNameSnapshot" HeaderText="יחידת אריזה" />
                                                                        <asp:BoundField DataField="IngredientCostSnapshot" HeaderText="עלות הרכיב במוצר" DataFormatString="{0:N2} ₪" HtmlEncode="false" />
                                                                    </columns>
                                                                </asp:GridView>
                                                            </div>
                                                            <asp:Panel ID="FirstCalculationMessagePanel" runat="server" CssClass="alert alert-secondary mt-3 mb-0" Visible='<%# Eval("PreviousCalculation") == null %>'>
                                                                זהו החישוב הראשון של המוצר, ולכן אין חישוב קודם להשוואה.
                                                            </asp:Panel>

                                                            <asp:Panel ID="CalculationChangeReasonsPanel" runat="server" CssClass="mt-3" Visible='<%# Eval("PreviousCalculation") != null %>'>

                                                                <h5 class="h6 mb-1">סיבות לשינוי לעומת החישוב הקודם
                                                                </h5>

                                                                <p class="text-secondary small mb-3">
                                                                    ההשוואה היא לחישוב מתאריך
                                                                    <%#: Eval("PreviousCalculation") == null? string.Empty: Convert.ToDateTime(Eval("PreviousCalculation.CalculatedAtUtc")).ToString("dd/MM/yyyy HH:mm") %>
                                                                </p>

                                                                <asp:Repeater ID="CalculationChangeReasonsRepeater" runat="server" DataSource='<%# Eval("ChangeReasons") %>' Visible='<%# HasCalculationChangeReasons(Eval("ChangeReasons")) %>'>

                                                                    <headertemplate>
                                                                        <div class="list-group">
                                                                    </headertemplate>

                                                                    <itemtemplate>
                                                                        <div class="list-group-item">
                                                                            <div class="d-flex flex-wrap justify-content-between gap-2">
                                                                                <strong>
                                                                                    <%#: Eval("IngredientName") %>
                                                                                </strong>

                                                                                <span class='<%# "fw-semibold "+ GetCostChangeCssClass(Eval("IngredientCostChange")) %>'>השפעה על העלות:
                                                                                    <%#: FormatSignedCostChange(Eval("IngredientCostChange")) %>
                                                                                </span>
                                                                            </div>

                                                                            <div class="text-secondary mt-1">
                                                                                <%#: FormatCalculationChangeReason(Container.DataItem) %>
                                                                            </div>
                                                                        </div>
                                                                    </itemtemplate>

                                                                    <footertemplate>
                                                        </div>
                                            </FooterTemplate>
                                        </asp:Repeater>

                                        <asp:Panel ID="NoCalculationChangeReasonsPanel" runat="server" CssClass="alert alert-light border mb-0"
                                            Visible='<%# !HasCalculationChangeReasons(Eval("ChangeReasons")) %>'>
                                            לא זוהה שינוי בנתוני הרכיבים לעומת החישוב הקודם.
                                        </asp:Panel>
                                </asp:Panel>
                            </div>
                </td>
                                                </tr>
            </itemtemplate>
        </asp:Repeater>
        </itemtemplate>

                                    <footertemplate>
                                        </tbody>
                                        </table>
                                    </footertemplate>
        </asp:Repeater>
    </div>
    <section class="mt-4" aria-labelledby='<%# "productCostChartHeading"+ Eval("ProductId") %>'>

        <h4 id='<%# "productCostChartHeading"+ Eval("ProductId") %>'
            class="h6 mb-3">מגמת העלות הכוללת:<%#: Eval("ProductName") %>
        </h4>

        <div class="d-flex flex-wrap align-items-center gap-2 mb-3">
            <strong class="fs-5">
                <%#: FormatCost(Eval("LatestTotalCost")) %>
            </strong>

            <span class='<%# "fw-semibold " +GetTrendCssClass(Eval("PeriodChangePercentage")) %>'><%#: FormatTrendChange(Eval("PeriodChangePercentage")) %>
            </span>
        </div>

        <div class="d-flex flex-wrap gap-2 mb-3" role="group" aria-label="בחירת טווח זמן לגרף">

            <asp:LinkButton ID="Last7DaysButton" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                Text="7 ימים" CommandName='<%# Convert.ToString(Eval("ProductId")) %>' CommandArgument="7d"
                CausesValidation="false" OnCommand="QuickDateRangeButton_Command" />

            <asp:LinkButton ID="Last30DaysButton" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                Text="30 ימים" CommandName='<%# Convert.ToString(Eval("ProductId")) %>' CommandArgument="30d"
                CausesValidation="false" OnCommand="QuickDateRangeButton_Command" />

            <asp:LinkButton ID="Last3MonthsButton" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                Text="3 חודשים" CommandName='<%# Convert.ToString(Eval("ProductId")) %>' CommandArgument="3m"
                CausesValidation="false" OnCommand="QuickDateRangeButton_Command" />

            <asp:LinkButton ID="Last6MonthsButton" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                Text="6 חודשים" CommandName='<%# Convert.ToString(Eval("ProductId")) %>' CommandArgument="6m"
                CausesValidation="false" OnCommand="QuickDateRangeButton_Command" />

            <asp:LinkButton ID="LastYearButton" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                Text="שנה" CommandName='<%# Convert.ToString(Eval("ProductId")) %>' CommandArgument="1y"
                CausesValidation="false" OnCommand="QuickDateRangeButton_Command" />

            <asp:LinkButton ID="AllHistoryButton" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                Text="הכל" CommandName='<%# Convert.ToString(Eval("ProductId")) %>' CommandArgument="all"
                CausesValidation="false" OnCommand="QuickDateRangeButton_Command" />

        </div>

        <div class="product-cost-point-comparison border rounded p-3 mb-3" role="status" aria-live="polite">

            <span class="product-cost-point-comparison-text text-secondary">ניתן לבחור בין שתי נקודות בגרף כדי להשוות ביניהן.
            </span>

            <strong class="product-cost-point-comparison-result me-2"></strong>
        </div>

        <input type="hidden" class="product-cost-chart-data"
            value='<%# System.Web.HttpUtility.HtmlAttributeEncode(GetProductCostChartDataJson(Eval("Calculations"))) %>' />

        <div class="product-cost-chart-container" style="position: relative; height: 20rem;">

            <canvas class="product-cost-history-chart" role="img"
                aria-label='<%# System.Web.HttpUtility.HtmlAttributeEncode("גרף היסטוריית העלות של "+ Convert.ToString(Eval("ProductName"))) %>'
                data-product-name='<%# System.Web.HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("ProductName"))) %>'></canvas>
        </div>
    </section>
    </div>
                    </div>
                </div>
            </itemtemplate>
        </asp:Repeater>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.5.1/dist/chart.umd.min.js"></script>
    <script src="<%: ResolveUrl("~/Scripts/calculation-history-chart.js") %>?v=2"></script>
</asp:Content>
