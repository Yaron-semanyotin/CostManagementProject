<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Recipe.aspx.cs" Inherits="CostWise.Recipe" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>מתכון</h1>
            <div>
                <asp:Label ID="ProductLabel" runat="server" AssociatedControlID="ProductDropDownList" Text="מוצר: " />
                <asp:DropDownList ID="ProductDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ProductDropDownList_SelectedIndexChanged">
                    <asp:ListItem Text="בחר מוצר" Value="" />
                </asp:DropDownList>
            </div>
            <asp:Panel ID="RecipeIngredientFormPanel" runat="server" Enabled="false">
                <h2>הוספת רכיב למתכון</h2>
                <div>
                    <asp:Label ID="IngredientLabel" runat="server" AssociatedControlID="IngredientDropDownList" Text="רכיב: " />
                    <asp:DropDownList ID="IngredientDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="IngredientDropDownList_SelectedIndexChanged">
                        <asp:ListItem Text="בחר רכיב" Value="" />
                    </asp:DropDownList>
                </div>
                <div>
                    <asp:Label ID="QuantityLabel" runat="server" AssociatedControlID="QuantityTextBox" Text="כמות במתכון: " />
                    <asp:TextBox ID="QuantityTextBox" runat="server" />
                </div>
                <div>
                    <asp:Label ID="MeasurementUnitLabel" runat="server" AssociatedControlID="MeasurementUnitDropDownList" Text="יחידת מידה: " />
                    <asp:DropDownList ID="MeasurementUnitDropDownList" runat="server">
                        <asp:ListItem Text="בחר יחידת מידה" Value="" />
                    </asp:DropDownList>
                </div>
                <div>
                    <asp:Button ID="AddRecipeIngredientButton" runat="server" Text="הוסף למתכון" OnClick="AddRecipeIngredientButton_Click" />
                </div>
            </asp:Panel>
            <asp:Label ID="ResultLabel" runat="server" />
            <h2>רכיבי המתכון</h2>
            <asp:GridView
                ID="RecipeIngredientsGrid"
                runat="server"
                AutoGenerateColumns="false"
                EmptyDataText="לא נמצאו רכיבים במתכון"
                DataKeyNames="RecipeIngredientId,IngredientId,MeasurementUnitId"
                OnRowEditing="RecipeIngredientsGrid_RowEditing"
                OnRowCancelingEdit="RecipeIngredientsGrid_RowCancelingEdit"
                OnRowUpdating="RecipeIngredientsGrid_RowUpdating"
                OnRowDeleting="RecipeIngredientsGrid_RowDeleting"
                OnRowDataBound="RecipeIngredientsGrid_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="רכיב">
                        <ItemTemplate>
                            <asp:Label ID="IngredientNameLabel" runat="server" Text='<%# Eval("IngredientName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="EditIngredientDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="EditIngredientDropDownList_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Quantity" HeaderText="כמות" />
                    <asp:TemplateField HeaderText="יחידת מידה">
                        <ItemTemplate>
                            <asp:Label ID="MeasurementUnitNameLabel" runat="server" Text='<%# Eval("MeasurementUnitName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="EditMeasurementUnitDropDownList" runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="ערוך" UpdateText="שמור" CancelText="ביטול" DeleteText="מחק" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
