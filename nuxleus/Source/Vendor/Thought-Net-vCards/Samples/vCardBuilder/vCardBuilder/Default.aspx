<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="DefaultPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>vCard Builder Demo</title>
    
    <style type="text/css">
         
      .Heading
      {
          background-color: silver;
          color: white;
          display: block;
          font-family: sans-serif;
          font-weight: bold;
          margin-bottom: 3px;
          padding: 4px;
          width: 100%;
      }
      
      .Help
      {
          background-color: #EFEFEF;
          border: 1px dashed silver;
          display: block;
          font-family: sans-serif;
          font-size: 10pt;
          padding: 4px;
      }
      
      .Label
      {
          color: black;
          display: block;
          font-family: sans-serif;
          font-size: small;
          margin-top: 10px;
      }      
      
    </style>
</head>
<body>
    <form id="PageForm" runat="server">
    
        <table width="600">
        
            <tr>
            
                <td valign="top" width="100">
                    <asp:Menu ID="MainMenu" runat="server" BackColor="#F7F6F3" DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#7C6F57" StaticSubMenuIndent="10px" OnMenuItemClick="MainMenu_MenuItemClick">
                        <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                        <DynamicHoverStyle BackColor="#7C6F57" ForeColor="White" />
                        <DynamicMenuStyle BackColor="#F7F6F3" />
                        <StaticSelectedStyle BackColor="#5D7B9D" />
                        <DynamicSelectedStyle BackColor="#5D7B9D" />
                        <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                        <Items>
                            <asp:MenuItem Text="Name" Value="Name"></asp:MenuItem>
                            <asp:MenuItem Text="Employment" Value="Employment"></asp:MenuItem>
                            <asp:MenuItem Text="Note" Value="Note"></asp:MenuItem>
                        </Items>
                        <StaticHoverStyle BackColor="#7C6F57" ForeColor="White" />
                    </asp:Menu>
                </td>
                <td valign="top">
                    <asp:MultiView ID="MainView" runat="server">
                        <asp:View ID="NameView" runat="server">
                            <asp:Label ID="NameHeading" runat="server" CssClass="Heading" EnableViewState="false" Text="Contact Name"></asp:Label>
                            <asp:Label ID="NameHelp" runat="server" CssClass="Help" EnableViewState="false" Text="Enter the given (first), family (last) and any midle names or initials."></asp:Label>
                            <asp:Label ID="GivenNameLabel" runat="server" AssociatedControlID="GivenName" CssClass="Label" EnableViewState="false" Text="Given (first) name:"></asp:Label>
                            <asp:TextBox ID="GivenName" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="FamilyNameLabel" runat="server" AssociatedControlID="FamilyName" CssClass="Label" EnableViewState="false" Text="Family (last) name:"></asp:Label>
                            <asp:TextBox ID="FamilyName" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="AdditionalNamesLabel" runat="Server" AssociatedControlID="AdditionalNames" CssClass="Label" EnableViewState="false" Text="Middle or additional names:"></asp:Label>
                            <asp:TextBox ID="AdditionalNames" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="NicknamesHelp" runat="server" CssClass="Label" EnableViewState="false" Text="Nicknames (separated by commas):"></asp:Label>
                            <asp:TextBox ID="Nicknames" runat="server" Columns="30" />
                            <asp:Label ID="NamePrefixLabel" runat="server" CssClass="Label" EnableViewState="false" Text="Prefix (e.g. Dr, Mr):"></asp:Label>
                            <asp:TextBox ID="NamePrefix" runat="server" Columns="30" />
                            <asp:Label ID="NameSuffixLabel" runat="server" CssClass="Label" EnableViewState="false" Text="Suffix (e.g. Jr.):"></asp:Label>
                            <asp:TextBox ID="NameSuffix" runat="server" Columns="30">
                            </asp:TextBox>
                        </asp:View>
                        <asp:View ID="EmploymentView" runat="server">
                            <asp:Label ID="EmploymentHeading" runat="server" CssClass="Heading" EnableViewState="false" Text="Employment"></asp:Label>
                            <asp:Label ID="EmploymentHelp" runat="server" CssClass="Help" Text="Enter the organization, job title, and a short description of the profession or role (e.g. 'Programmer' or 'Executive')."></asp:Label>
                            <asp:Label ID="OrganizationLabel" runat="server" AssociatedControlID="Organization" CssClass="Label" EnableViewState="false" Text="Organization:"></asp:Label>
                            <asp:TextBox ID="Organization" runat="Server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="TitleLabel" runat="server" AssociatedControlID="Title" CssClass="Label" EnableViewState="false" Text="Job title:"></asp:Label>
                            <asp:TextBox ID="Title" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="RoleLabel" runat="server" AssociatedControlID="Role" CssClass="Label" EnableViewState="false" Text="Role/Profession:"></asp:Label>
                            <asp:TextBox ID="Role" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="WorkEmailLabel" runat="server" AssociatedControlID="WorkEmail" CssClass="Label" EnableViewState="false" Text="Work email:"></asp:Label>
                            <asp:TextBox ID="WorkEmail" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="WorkPhoneLabel" runat="Server" AssociatedControlID="WorkPhone" CssClass="Label" EnableViewState="false" Text="Work Phone:"></asp:Label>
                            <asp:TextBox ID="WorkPhone" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="WorkFaxLabel" runat="server" AssociatedControlID="WorkFax" CssClass="Label" EnableViewState="false" Text="Work Fax:"></asp:Label>
                            <asp:TextBox ID="WorkFax" runat="server" Columns="30">
                            </asp:TextBox>
                            <asp:Label ID="WorkWebSiteLabel" runat="server" AssociatedControlID="WorkWebSite" CssClass="Label" EnableViewState="false" Text="Web site:"></asp:Label>
                            <asp:TextBox ID="WorkWebSite" runat="server" Columns="30">
                            </asp:TextBox>
                        </asp:View>
                        <asp:View ID="NoteView" runat="server">
                            <asp:Label ID="NoteHeading" runat="server" CssClass="Heading" EnableViewState="false" Text="Note"></asp:Label>
                            <asp:Label ID="NoteHelp" runat="server" CssClass="Help" Text="Enter a note about this person."></asp:Label>
                            <asp:TextBox ID="Note" runat="server" TextMode="MultiLine" Rows="10" />
                        </asp:View>
                    </asp:MultiView>
                </td>
            </tr>
        </table>
        
   
    <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" />
        
    </form>
</body>
</html>
