<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="CostWise.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body data-clear-product-builder-cache="true">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="UsernameLabel" runat="server" AssociatedControlID="UsernameTextBox" Text="שם משתמש: " />
            <asp:TextBox ID="UsernameTextBox" runat="server" MaxLength="50" />
        </div>
        <div>
            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="PasswordTextBox" Text="סיסמה: " />
            <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" MaxLength="128" />
        </div>
        <div>
            <asp:Button ID="LoginButton" runat ="server" Text="התחברות" OnClick="LoginButton_Click" />
        </div>
        <div>
            <asp:Label ID="LoginResultLabel" runat="server" />
        </div>
    </form>
    <script src="<%: ResolveUrl("~/Scripts/product-builder.js") %>"></script>
</body>
</html>
