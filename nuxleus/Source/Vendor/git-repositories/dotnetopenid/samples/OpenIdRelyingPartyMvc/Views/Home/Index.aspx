﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1>OpenID Relying Party </h1>
	<h2>Provided by <a href="http://dotnetopenid.googlecode.com">DotNetOpenAuth</a> </h2>
	<% if (User.Identity.IsAuthenticated) { %>
	<p><b>You are already logged in!</b> Try visiting the
		<%=Html.ActionLink("Members Only", "Index", "User") %>
		area. </p>
	<% } else { %>
	<p>Visit the
		<%=Html.ActionLink("Members Only", "Index", "User") %>
		area to trigger a login. </p>
	<p>Optionally, you can try out the <%=Html.ActionLink("JQuery login popup UX", "LoginPopup", "User")%>. </p>
	<% } %>
</asp:Content>
