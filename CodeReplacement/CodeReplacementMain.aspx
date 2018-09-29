<%@ Page Title="" Language="C#" MasterPageFile="~/CodeReplacementMaster.Master" AutoEventWireup="true" CodeBehind="CodeReplacementMain.aspx.cs" Inherits="CodeReplacement.CodeReplacementNBA" %>
<script runat="server">
void SetAltHomeTeamName(object sender, EventArgs e)
{
    HomeTeamAlternateDesc.Text = HomeTeamDropDown.SelectedItem.Text;
    HomeTeamPrefix.Focus();
}

void SetAltVisitingTeamName(object sender, EventArgs e)
{
    VisitingTeamAlternateDesc.Text = VisitingTeamDropDown.SelectedItem.Text;
    VisitingTeamDropDown.Focus();
}
    
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style3
        {
            text-align: left;
            font-family: Calibri;
        }
        .style4
        {
            text-align: right;
            font-family: Calibri;
            color: #000000;
        }
        .style4_under
        {
            text-align: right;
            font-family: Calibri;
            color: #000000;
            text-decoration: underline;
        }
        .style6
        {
            font-family: Calibri;
        }
        .style7
        {
            font-family: Calibri;
            font-size: small;
        }
        .style8
        {
            text-align: right;
            font-family: Calibri;
            color: #000000;
            width: 610px;
            font-size: small;
        }
        .style9
    {
        text-align: right;
        font-family: Calibri;
        color: #000000;
        width: 610px;
        font-size: medium;
    }
        </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblSport" runat="server" 
        style="font-family: Calibri; font-size: xx-large" ForeColor="Gray"></asp:Label>
    <span class="style4">
    <br />
    <br />
    <br />
    Home Team:&nbsp; </span>
    <asp:DropDownList ID="HomeTeamDropDown" runat="server" Height="22px" 
            Width="242px" CssClass="style3" 
        OnSelectedIndexChanged="SetAltHomeTeamName" AutoPostBack="True">
            <asp:ListItem></asp:ListItem>
    </asp:DropDownList>
    <span class="style4">&nbsp;&nbsp;
        </span>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
            ControlToValidate="HomeTeamDropDown" ErrorMessage="Home Team is Required" 
            CssClass="style4"></asp:RequiredFieldValidator>
    <br class="style4" /><span class="style4">Home Team Prefix:&nbsp;
        </span>
    <asp:TextBox ID="HomeTeamPrefix" runat="server" MaxLength="5" Width="65px" 
            CssClass="style3"></asp:TextBox>
        &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
            ControlToValidate="HomeTeamPrefix" 
            ErrorMessage="Home Team Prefix is required" CssClass="style4"></asp:RequiredFieldValidator>
    <br class="style4" /><span class="style4">Home Team Name:&nbsp; </span>
    <asp:TextBox ID="HomeTeamAlternateDesc" runat="server" CssClass="style3" 
            Width="200px"></asp:TextBox>
    <span class="style4">&nbsp;</span><span class="style8"> (if other than as listed in 
    Home Team list)<br />
</span><span class="style4"> Home Team Coach:&nbsp; </span>
<asp:TextBox ID="HomeTeamCoach" runat="server" Width="200px" CssClass="style3"></asp:TextBox>
<br 
        class="style4" />
    <p>
        <span class="style4">Visiting Team:&nbsp; </span>
        <asp:DropDownList ID="VisitingTeamDropDown" runat="server" Height="22px" 
            Width="242px" CssClass="style3" 
            OnSelectedIndexChanged="SetAltVisitingTeamName" AutoPostBack="True">
            <asp:ListItem></asp:ListItem>
        </asp:DropDownList>
        <span class="style4">&nbsp;&nbsp;
        </span>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
            ControlToValidate="VisitingTeamDropDown" 
            ErrorMessage="Visiting Team is Required" CssClass="style4"></asp:RequiredFieldValidator>
        <br class="style4" /><span class="style4">Visiting Team Prefix:&nbsp;
        </span>
        <asp:TextBox ID="VisitingTeamPrefix" runat="server" MaxLength="5" Width="65px" 
            CssClass="style3"></asp:TextBox>
&nbsp;
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
            ControlToValidate="VisitingTeamPrefix" 
            ErrorMessage="Visiting Team Prefix is required" CssClass="style4"></asp:RequiredFieldValidator>
        <br class="style4" /><span class="style4">Visiting Team Name:&nbsp; </span>
        <asp:TextBox ID="VisitingTeamAlternateDesc" runat="server" CssClass="style3" 
                Width="200px"></asp:TextBox>
        <span class="style4">&nbsp;</span><span class="style8"> (if other than as listed in 
        Visiting Team list)<br />
        </span><span class="style4"> Visiting Team Coach: </span><span class="style8"> &nbsp;<asp:TextBox 
            ID="VisitingTeamCoach" runat="server" Width="200px" CssClass="style3"></asp:TextBox>
        </span></p>
    <br />
&nbsp;<p>
        <asp:Label ID="lblDuplicateText" runat="server" style="font-family: Calibri" 
            Text="Duplicate Text:  " Visible="False"></asp:Label>
        <asp:TextBox ID="txtDuplicate" runat="server" style="font-family: Calibri" 
            Visible="False" Width="200px" CssClass="style3">@@@@ DUPLICATE </asp:TextBox>
&nbsp;</p>
    <p>
        <asp:Button ID="btnCreateFile" runat="server" 
            Text="Create Code Replacement File" onclick="btnCreateFile_Click" 
                style="height: 26px" CssClass="style3" />
    </p>
<p>
        <asp:Button ID="btnEditPlayerList" runat="server" 
             Text="Edit Roster" onclick="btnEditPlayerList_Click" 
            style="height: 26px" CssClass="style3" />
    </p>
    <p>
        <asp:HyperLink ID="FileLink" runat="server" Visible="False" CssClass="style4">Download Code Replacement File</asp:HyperLink>
        <br />
        <asp:Label ID="lblSaveDirection" runat="server" 
            style="font-family: Calibri; font-size: small" 
            Text="Right Click + Save As / Right Click + Save Link As to Save the file to your computer" 
            Visible="False"></asp:Label>
    </p>
</asp:Content>
