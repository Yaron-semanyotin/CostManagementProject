<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="CostWise.Products" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>מוצרים</h1>
            <h2>הוספת מוצר</h2>
            <div>
                <asp:Label ID="ProductNameLabel" runat="server" AssociatedControlID="ProductNameTextBox" Text="שם המוצר: " />
                <asp:TextBox ID="ProductNameTextBox" runat="server" />
            </div>
            <div>
                <asp:Label ID="YieldQuantityLabel" runat="server" AssociatedControlID="YieldQuantityTextBox" Text="כמות תוצר: " />
                <asp:TextBox ID="YieldQuantityTextBox" runat="server" />
            </div>
            <div>
                <asp:Label ID="YieldUnitLabel" runat="server" AssociatedControlID="YieldUnitDropDownList" Text="יחידת תוצר: " />
                <asp:DropDownList ID="YieldUnitDropDownList" runat="server">
                    <asp:ListItem Text="בחר יחידת מידה" Value="" />
                </asp:DropDownList>
            </div>
            <div>
                <asp:Button ID="AddProductButton" runat="server" Text="הוסף מוצר" OnClick="AddProductButton_Click" />
            </div>
            <asp:Label ID="ResultLabel" runat="server" />
            <asp:GridView ID="ProductsGrid" runat="server" AutoGenerateColumns="false" EmptyDataText="לא נמצאו מוצרים" DataKeyNames="ProductId,IsActive" OnRowEditing="ProductsGrid_RowEditing" OnRowCancelingEdit="ProductsGrid_RowCancelingEdit" OnRowUpdating="ProductsGrid_RowUpdating" OnRowDeleting="ProductsGrid_RowDeleting" OnRowDataBound="ProductsGrid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="ProductName" HeaderText="שם המוצר" />
                    <asp:BoundField DataField="YieldQuantity" HeaderText="כמות תוצר" />
                    <asp:TemplateField HeaderText="יחידת תוצר">
                        <ItemTemplate>
                            <asp:Label ID="YieldUnitNameLabel" runat="server" Text='<%# Eval("YieldUnitLabel") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="EditYieldUnitDropDownList" runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="IsActive" HeaderText="פעיל" ReadOnly="true" />
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" EditText="ערוך" UpdateText="שמור" CancelText="ביטול" DeleteText="השבת" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
