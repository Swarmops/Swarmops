<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="Activizr.Logic.Structure"%>

<%
    int organizationId = Convert.ToInt32(Request.QueryString["OrganizationId"]);
    int memberCount = Organization.FromIdentity(organizationId).GetTree().GetMemberCount();

    string variableName = Request.QueryString["VariableName"];
    
    if (String.IsNullOrEmpty (variableName))
    {
    	variableName = "membercount";
    }
    
	Response.ContentType="text/plain";
	Response.Write ("<?php $" + variableName + " = '" + memberCount.ToString() + "'; ?>"); 
%>