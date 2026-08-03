<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MeasurementUnits.aspx.cs" Inherits="CostWise.MeasurementUnits" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>יחידות מידה</h1>
            <asp:Label ID="ResultLabel" runat="server" />

            <h2>הוספת יחידה מותאמת</h2>
            <div>
                <asp:Label ID="CustomUnitNameLabel" runat="server" AssociatedControlID="CustomUnitNameTextBox" Text="שם היחידה: " />
                <asp:TextBox ID="CustomUnitNameTextBox" runat="server" MaxLength="50" />
            </div>
            <div>
                <asp:Label ID="UnitFamilyLabel" runat="server" AssociatedControlID="UnitFamilyDropDownList" Text="משפחת היחידה: " />
                <asp:DropDownList ID="UnitFamilyDropDownList" runat="server">
                    <asp:ListItem Text="בחר משפחה" Value="" />
                    <asp:ListItem Text="משקל" Value="Weight" />
                    <asp:ListItem Text="נפח" Value="Volume" />
                    <asp:ListItem Text="כמות" Value="Quantity" />
                </asp:DropDownList>
            </div>
            <div>
                <asp:Label ID="ConversionFactorLabel" runat="server" AssociatedControlID="ConversionFactorTextBox" Text="מקדם המרה ליחידת בסיס: " />
                <asp:TextBox ID="ConversionFactorTextBox" runat="server" />
            </div>
            <div>
                <asp:Button ID="AddCustomUnitButton" runat="server" Text="הוסף יחידה" OnClick="AddCustomUnitButton_Click" />
            </div>

            <h2>יחידות מערכת</h2>
            <asp:GridView ID="SystemUnitsGrid" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="UnitName" HeaderText="שם היחידה" />
                    <asp:BoundField DataField="UnitFamily" HeaderText="משפחה" />
                    <asp:BoundField DataField="ConversionFactorToBase" HeaderText="מקדם המרה ליחידת בסיס" />
                </Columns>
            </asp:GridView>

            <h2>יחידות מותאמות לעסק</h2>
            <asp:GridView ID="CustomUnitsGrid" runat="server" AutoGenerateColumns="false"
                EmptyDataText="לא הוגדרו יחידות מותאמות" DataKeyNames="MeasurementUnitId" OnRowEditing="CustomUnitsGrid_RowEditing"
                OnRowCancelingEdit="CustomUnitsGrid_RowCancelingEdit" OnRowUpdating="CustomUnitsGrid_RowUpdating">
                <Columns>
                    <asp:BoundField DataField="UnitName" HeaderText="שם היחידה" />
                    <asp:BoundField DataField="UnitFamily" HeaderText="משפחה" />
                    <asp:BoundField DataField="ConversionFactorToBase" HeaderText="מקדם המרה ליחידת בסיס" />
                    <asp:CommandField ShowEditButton="true" EditText="ערוך" UpdateText="שמור" CancelText="ביטול" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
