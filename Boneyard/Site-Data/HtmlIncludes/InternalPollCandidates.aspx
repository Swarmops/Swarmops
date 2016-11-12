<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InternalPollCandidates.aspx.cs" Inherits="HtmlIncludes_InternalPollCandidates" %>
<%@ Import Namespace="Activizr.Logic.Pirates"%>
<%@ Import Namespace="Activizr.Logic.Structure"%>


    <form id="form1" runat="server">
    <div>
        <asp:Repeater ID="RepeaterCandidates" runat="server">
        <ItemTemplate>
            <div class="DivCandidatePhoto"><a href="http://data.piratpartiet.se/Handlers/DisplayPortrait.aspx?PersonId=<%# Eval("PersonId") %>"><img src="http://data.piratpartiet.se/Handlers/DisplayPortrait.aspx?PersonId=<%# Eval("PersonId") %>&YSize=128" border="0"/></a></div>
            <span class="SpanCandidateName"><%# ((Person) Eval("Person")).Canonical %></span><br />
            <span class="SpanCandidateSubtitle"><%# Eval("PersonSubtitle") %></span><br />
            <span class="SpanCandidateBlog">Blogg: <a href="<%# ((Person) Eval("Person")).BlogUrl %>"><%# ((Person) Eval("Person")).BlogName %></a></span><br />

            <span class="SpanCandidacyStatement"><%# Eval("CandidacyStatement") %></span><br />
        </ItemTemplate>
        <SeparatorTemplate><hr class="CandidateSepator" /></SeparatorTemplate>
        </asp:Repeater>
    </div>
    </form>

