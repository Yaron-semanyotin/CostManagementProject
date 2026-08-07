<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalculationHistory.aspx.cs" Inherits="CostWise.CalculationHistory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>היסטוריית חישובים</h1>
            <asp:Label ID="ResultLabel" runat="server" />
            <asp:GridView ID="CalculationsGrid" runat="server" AutoGenerateColumns="false" EmptyDataText="לא נמצאו חישובים היסטוריים">
                <Columns>
                    <asp:BoundField DataField="CostCalculationId" HeaderText="מספר חישוב" />
                    <asp:BoundField DataField="ProductNameSnapshot" HeaderText="מוצר" />
                    <asp:BoundField DataField="YieldQuantitySnapshot" HeaderText="כמות תוצר" />
                    <asp:BoundField DataField="YieldUnitLabelSnapshot" HeaderText="יחידת תוצר" />
                    <asp:BoundField DataField="TotalIngredientCostSnapshot" HeaderText="עלות כוללת" />
                    <asp:BoundField DataField="CostPerYieldUnitSnapshot" HeaderText="עלות ליחידת תוצר" />
                    <asp:BoundField DataField="CalculatedAtUtc" HeaderText="מועד החישוב" />
                    <asp:HyperLinkField HeaderText="פרטים" Text="הצג פרטים" DataNavigateUrlFields="CostCalculationId" DataNavigateUrlFormatString="CalculationDetails.aspx?calculationId={0}" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
