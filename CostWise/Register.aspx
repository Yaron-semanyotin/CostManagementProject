<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="CostWise.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="BusinessNameLabel" runat="server" AssociatedControlID="BusinessNameTextBox" Text="שם העסק: " />
            <asp:TextBox ID="BusinessNameTextBox" runat="server" MaxLength="150" />
        </div>
        <div>
            <asp:Label ID="UsernameLabel" runat="server" AssociatedControlID="UsernameTextBox" Text="שם משתמש: " />
            <asp:TextBox ID="UsernameTextBox" runat="server" MaxLength="50" />
        </div>
        <div>
            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="PasswordTextBox" Text="סיסמה: " />
            <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" />
        </div>
        <div>
            <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPasswordTextBox" Text="אימות סיסמה: " />
            <asp:TextBox ID="ConfirmPasswordTextBox" runat="server" TextMode="Password" />
        </div>
        <div>
            <asp:Button ID="RegisterButton" runat="server" Text="הרשמה" OnClick="RegisterButton_Click" />
        </div>
        <div>
            <asp:Label ID="RegistrationResultLabel" runat="server" />
        </div>
    </form>
</body>
</html>
