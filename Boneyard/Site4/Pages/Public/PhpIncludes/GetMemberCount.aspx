<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="Activizr.Logic.Structure"%>

<%
    int organizationId = Convert.ToInt32(Request.QueryString["OrganizationId"]);
    int memberCount = Organization.FromIdentity(organizationId).GetMemberCount();
	
	Response.ContentType="text/plain";
	Response.Write ("<?php $membercount = '" + memberCount.ToString() + "'; ?>"); 
%>