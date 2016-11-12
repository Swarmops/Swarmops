<%@ Page Language="C#" %>
<%@ Import Namespace="Activizr.Logic.Pirates"%>

<%= Memberships.GetMemberCountForOrganizationSince(Activizr.Logic.Structure.Organization.PPSEid, new DateTime(2008, 12, 1))%>