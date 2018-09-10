<%@ Page Title="" Language="C#" MasterPageFile="~/CodeReplacementMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CodeReplacement.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style3
        {
            font-size: xx-large;
            font-family: Calibri;
        }
        .style4
        {
            font-family: Calibri;
            font-size: xx-large;
        }
        .style5
        {
            font-family: Calibri;
        }
        .style6
        {
            font-family: Calibri;
            font-size: xx-large;
            width: 610px;
        }
        .style7
        {
            font-family: Calibri;
            font-size: xx-large;
            width: 610px;
            text-align: left;
            color: #808080;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <span class="style7">PhotoMechanic Code Replacements</span><span class="style3"><br />
    <br />
    </span>
    <table class="style1">
        <tr>
            <td>
                <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click" 
                    CssClass="style5">NBA</asp:LinkButton>
            </td>
            <td>
                <asp:LinkButton ID="LinkButton2" runat="server" onclick="LinkButton2_Click" 
                    CssClass="style5">MLB</asp:LinkButton>
            </td>
            <td>
                <asp:LinkButton ID="LinkButton3" runat="server" onclick="LinkButton3_Click" 
                    CssClass="style5">NFL</asp:LinkButton>
            </td>
            <td class="style5">
                <asp:LinkButton ID="LinkButton4" runat="server" onclick="LinkButton4_Click">NCAA Football</asp:LinkButton>
            </td>
            <td>
                <asp:LinkButton ID="LinkButton5" runat="server" onclick="LinkButton5_Click" 
                    style="font-family: Calibri">NCAA Basketball (Mens)</asp:LinkButton>
            </td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <br />
</asp:Content>
