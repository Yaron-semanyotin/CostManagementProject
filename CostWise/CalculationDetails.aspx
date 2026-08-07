<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CalculationDetails.aspx.cs" Inherits="CostWise.CalculationDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>פרטי חישוב היסטורי</h1>
            <asp:Label ID="ResultLabel" runat="server" />
            <asp:Panel ID="CalculationPanel" runat="server" Visible="false">
                <p>
                    מוצר:<asp:Label ID="ProductNameLabel" runat="server" />
                </p>
                <p>
                    תפוקה:<asp:Label ID="YieldLabel" runat="server" />
                </p>
                <p>
                    עלות כוללת:<asp:Label ID="TotalCostLabel" runat="server" />
                </p>
                <p>
                    עלות ליחידת תוצר:<asp:Label ID="CostPerYieldUnitLabel" runat="server" />
                </p>
                <p>
                    מועד החישוב:<asp:Label ID="CalculatedAtLabel" runat="server" />
                </p>
                <asp:GridView
                    ID="CalculationItemsGrid"
                    runat="server"
                    AutoGenerateColumns="false"
                    EmptyDataText="לא נמצאו פריטי חישוב">
                    <Columns>
                        <asp:BoundField
                            DataField="SortOrderSnapshot"
                            HeaderText="סדר" />

                        <asp:BoundField
                            DataField="IngredientNameSnapshot"
                            HeaderText="רכיב" />

                        <asp:BoundField
                            DataField="PackagePriceSnapshot"
                            HeaderText="מחיר אריזה" />

                        <asp:BoundField
                            DataField="PackageQuantitySnapshot"
                            HeaderText="כמות באריזה" />

                        <asp:BoundField
                            DataField="PackageUnitNameSnapshot"
                            HeaderText="יחידת אריזה" />

                        <asp:BoundField
                            DataField="PackageUnitFamilySnapshot"
                            HeaderText="משפחת יחידת אריזה" />

                        <asp:BoundField
                            DataField="PackageUnitConversionFactorSnapshot"
                            HeaderText="מקדם יחידת אריזה" />

                        <asp:BoundField
                            DataField="RecipeQuantitySnapshot"
                            HeaderText="כמות במתכון" />

                        <asp:BoundField
                            DataField="RecipeUnitNameSnapshot"
                            HeaderText="יחידת מתכון" />

                        <asp:BoundField
                            DataField="RecipeUnitFamilySnapshot"
                            HeaderText="משפחת יחידת מתכון" />

                        <asp:BoundField
                            DataField="RecipeUnitConversionFactorSnapshot"
                            HeaderText="מקדם יחידת מתכון" />

                        <asp:BoundField
                            DataField="BaseUnitNameSnapshot"
                            HeaderText="יחידת בסיס" />

                        <asp:BoundField
                            DataField="PackageQuantityInBaseUnitSnapshot"
                            HeaderText="כמות אריזה ביחידת בסיס" />

                        <asp:BoundField
                            DataField="RecipeQuantityInBaseUnitSnapshot"
                            HeaderText="כמות מתכון ביחידת בסיס" />

                        <asp:BoundField
                            DataField="PricePerBaseUnitSnapshot"
                            HeaderText="מחיר ליחידת בסיס" />

                        <asp:BoundField
                            DataField="IngredientCostSnapshot"
                            HeaderText="עלות הרכיב" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
