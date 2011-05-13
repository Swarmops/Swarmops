<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="Activizr.Logic.Structure"%>

<%@ Import Namespace="Activizr.Interface.Objects" %>
<%
    OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);

    int memberCount = 0;

    if (metadata.Recursive)
    {
        memberCount = Organization.FromIdentity(metadata.OrganizationId).GetTree().GetMemberCount();
    }
    else
    {
        memberCount = Organization.FromIdentity(metadata.OrganizationId).GetMemberCount();
    }

    string variableName = Request.QueryString["VariableName"];

    if (String.IsNullOrEmpty(variableName))
    {
        variableName = "membercount";
    }

    Response.ContentType = "text/plain";
    Response.Write("<?php $" + variableName + " = '" + memberCount.ToString() + "'; ?>"); 
%>