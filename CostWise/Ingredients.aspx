<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Ingredients.aspx.cs" Inherits="CostWise.Ingredients" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>רכיבים</h1>
            <asp:Label ID="ResultLabel" runat="server" />
            <h2>הוספת רכיבים</h2>
            <div>
                <asp:Label ID="IngredientNameLabel" runat="server" AssociatedControlID="IngredientNameTextBox" Text="שם הרכיב: " />
                <asp:TextBox ID="IngredientNameTextBox" runat="server" />
            </div>
            <div>
                <asp:Label ID="PackagePriceLabel" runat="server" AssociatedControlID="PackagePriceTextBox" Text="מחיר האריזה: " />
                <asp:TextBox ID="PackagePriceTextBox" runat="server" />
            </div>
            <div>
                <asp:Label ID="PackageQuantityLabel" runat="server" AssociatedControlID="PackageQuantityTextBox" Text="כמות באריזה: " />
                <asp:TextBox ID="PackageQuantityTextBox" runat="server" />
            </div>
            <div>
                <asp:Label ID="PackageUnitLabel" runat="server" AssociatedControlID="PackageUnitDropDownList" Text="יחידת האריזה: " />
                <asp:DropDownList ID="PackageUnitDropDownList" runat="server">
                    <asp:ListItem Text="בחר יחידת מידה" Value="" />
                </asp:DropDownList>
            </div>
            <div>
                <asp:Button ID="AddIngredientButton" runat="server" Text="הוסף רכיב" OnClick="AddIngredientButton_Click" />
            </div>
            <asp:GridView ID="IngredientsGrid" runat="server" AutoGenerateColumns="false" EmptyDataText="לא נמצאו רכיבים"
                DataKeyNames="IngredientId,IsActive" OnRowEditing="IngredientsGrid_RowEditing"
                OnRowCancelingEdit="IngredientsGrid_RowCancelingEdit" OnRowUpdating="IngredientsGrid_RowUpdating"
                OnRowDataBound="IngredientsGrid_RowDataBound" OnRowDeleting="IngredientsGrid_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="IngredientName" HeaderText="שם הרכיב" />
                    <asp:BoundField DataField="PackagePrice" HeaderText="מחיר האריזה" />
                    <asp:BoundField DataField="PackageQuantity" HeaderText="כמות באריזה" />
                    <asp:TemplateField HeaderText="יחידת האריזה">
                        <ItemTemplate>
                            <asp:Label ID="PackageUnitNameLabel" runat="server" Text='<%#Eval("PackageUnitName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="EditPackageUnitDropDownList" runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="IsActive" HeaderText="פעיל" ReadOnly="true" />
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="ערוך" UpdateText="שמור" CancelText="ביטול" DeleteText="השבת" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
