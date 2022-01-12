<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CIFIDAccountInfoReport.aspx.cs" Inherits="ABankAdmin.RDLCReports.Aspx.CIFIDAccountInfoReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   <form id="formCIFIDAccountReportViewer" runat="server">  
    <div>  
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>  
        <rsweb:ReportViewer ID="CIFIDAccountInfoReportViewer" runat="server" Width="100%"></rsweb:ReportViewer>  
    </div>  
</form>  
</body>
</html>