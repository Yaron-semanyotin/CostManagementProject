<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MeasurementUnitTest.aspx.cs" Inherits="CostWise.MeasurementUnitTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="ResultLabel" runat="server" />
            <asp:GridView ID ="AvailableUnitsGrid" runat="server" AutoGenerateColumns="true" />
        </div>
    </form>
</body>
</html>
