<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="editIntroBlock.ascx.cs" Inherits="cec_publicweb.input.usrctrls.editIntroBlock" %>

<div runat="server">
    <div class="row col-sm-12">
        <%-- visible when cohort rep is accessing their draft record --%>
        <div id="draft_intro" runat="server">
            <h2 class="pg-title">Cohort Forms</h2>
            <p>Thank you for taking the time to complete this form. The information you provide will populate the Cancer Epidemiology Descriptive Cohort Database (<a href="http://CEDCD.nci.nih.gov">http://CEDCD.nci.nih.gov</a>). Users of the CEDCD can access information about Cancer Epidemiology Centers; compare cohort characteristics and types of data collected; and tabulate counts of participants, cancer endpoints, and biospecimens. We hope you will find the CEDCD useful in identifying potential collaborators and facilitating future studies.  If you have questions, please contact the CEDCD Helpdesk or Dr. Joanne Elena (<a href="mailto:elenajw@mail.nih.gov">elenajw@mail.nih.gov</a>) directly.</p>
        </div>

        <%-- visible when cohort rep is accessing their pending record --%>
        <div id="pending_intro" runat="server">
            <h2 class='pg-title'>Cohort Forms: Pending Review</h2>
            <div class='well'>This form is pending NCI review, please come back at a later time.</div>
        </div>

        <%-- Visible as reviewer or higher --%>
        <div id="reviewer_intro" runat="server">
            <h2 class="pg-title">Cohort Forms</h2>
            <div class="well change-well clearfix">
                <div class="col-sm-2">Changes (<span id="change_count" runat="server" />)</div>
                <div class="col-sm-10" id="list_changes" runat="server">

                </div>
            </div>
        </div>

        <%-- visible when reviewer on published or unpublished records --%>
        <div id="published_intro" runat="server">
            <h2 class='pg-title'>Cohort Forms: Public Record View</h2>
            <div class='well'>This is a published record and cannot be changed. Click Start Draft to change this cohort's data.</div>
        </div>
    </div>
</div>