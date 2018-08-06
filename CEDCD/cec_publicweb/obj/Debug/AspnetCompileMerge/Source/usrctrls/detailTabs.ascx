<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="detailTabs.ascx.cs" Inherits="cec_publicweb.usrctrls.detailTabs" %>

<div id="cohortDetailTabs">
<ul class="nav nav-tabs">
    <li runat="server" role="presentation"><asp:LinkButton ID="btnBasic" runat="server"  CommandArgument="basic" CommandName="compare" Text="Basic Info" /></li>
    <li runat="server" role="presentation"><asp:LinkButton ID="btnBaseline" runat="server"  CommandArgument="baseline" CommandName="compare" Text="Baseline Data" /></li>
    <li runat="server" role="presentation"><asp:LinkButton ID="btnFollowup" runat="server"  CommandArgument="followup" CommandName="compare" Text="Followup Data" /></li>   
    <li runat="server" role="presentation"><asp:LinkButton ID="btnCancerInfo" runat="server"  CommandArgument="cancer" CommandName="compare" Text="Cancer Info" /></li>
    <li runat="server" role="presentation"><asp:LinkButton ID="btnMortality" runat="server"  CommandArgument="mortality" CommandName="compare" Text="Mortality" /></li>
    <li runat="server" role="presentation"><asp:LinkButton ID="btnLinkages" runat="server"  CommandArgument="linkages" CommandName="compare" Text="Linkages and Technology" /></li>
    <li runat="server" role="presentation"><asp:LinkButton ID="btnSpecimen" runat="server"  CommandArgument="specimen" CommandName="compare" Text="Specimen Overview" /></li>
</ul>
</div><!--cohortDetailTabs-->