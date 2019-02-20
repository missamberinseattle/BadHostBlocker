<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IpWhiteLister._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>IpWhiteLister</title>
    <meta name="viewport" content="width=500; initial-scale=1" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="InputDiv" runat="server">
        <div class="Instructions">Greetings! Please enter the code to add <span id="IpAddress" runat="server">1.2.3.4</span> to the host white list.</div>
        <input type="password" id="Passcode" class="input password" runat="server" />
        <asp:Button ID="Go" runat="server" Text="Continue" onclick="Go_Click" />
        <div id="ErrorMessage" class="error" runat="server" visible="false"></div>
    </div>
    <div id="StatusDiv" runat="server" visible="false">
        <div id="StatusMessage" runat="server"></div>
    </div>
    </form>
</body>
</html>
