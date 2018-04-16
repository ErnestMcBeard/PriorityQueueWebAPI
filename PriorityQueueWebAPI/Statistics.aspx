<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Statistics.aspx.cs" Inherits="PriorityQueueWebAPI.Statistics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Statistics</h2>
            <label>Date:</label>
            <asp:Label ID="DateError" Text="Please enter a valid date" Visible="false" runat="server" style="margin-left:15px; color:red;"/>
            <div style="margin-top: 5px; margin-bottom:15px;">
                <asp:TextBox runat="server" ID="Month" Width="30" MaxLength="2" placeholder="MM"/>
                <asp:TextBox runat="server" ID="Day" Width="30" MaxLength="2" placeholder="dd"/>
                <asp:TextBox runat="server" ID="Year" Width="50" MaxLength="4" placeholder="yyyy"/>
            </div>
            <div>
                <asp:Button Text="Generate (Day)" runat="server" OnClick="GenerateDay_Click"/>
                <asp:Button Text="Generate (Month)" runat="server" OnClick="GenerateMonth_Click"/>
            </div>
        </div>
    </form>
</body>
</html>
