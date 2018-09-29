<%@ Page Title="" Language="C#" MasterPageFile="~/CodeReplacementMaster.Master" AutoEventWireup="true" CodeBehind="EditRosters.aspx.cs" Inherits="CodeReplacement.EditRosters" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblTeams" runat="server" CssClass="style2" Text="Matchup:"></asp:Label>
    <br />
    <asp:HyperLink ID="HyperLink2" runat="server" Target="_blank" class="style6" NavigateUrl="EditSample.txt">Sample</asp:HyperLink>
    <br />
    <asp:Button ID="btnCreateCodeReplacementFile" runat="server" CssClass="style3" 
        onclick="btnCreateCodeReplacementFile_Click" 
        Text="Create Code Replacement File" />
    <asp:TextBox ID="txtRosters" runat="server" Height="600px" TextMode="MultiLine" 
        Width="100%" AutoPostBack="True" ontextchanged="txtRosters_TextChanged"></asp:TextBox>
</asp:Content>
