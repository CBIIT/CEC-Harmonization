<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="cohortDetails.aspx.cs" Inherits="cec_publicweb.cohortDetails" %>
<%@ Register Src="~/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/nci-header.ascx" TagName="nciheader" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/contact.ascx" TagName="contact" TagPrefix="usr" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
<head runat="server">
<title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />

<!-- style links -->
<link href="/css/bootstrap.css" rel="Stylesheet" type="text/css" />
<link href="/css/main.css" rel="stylesheet" type="text/css" />

<!-- web fonts -->
<link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />

<!-- javascript -->
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
<script src="/scripts/js_omni.js" type="text/javascript"></script>
<script src="/scripts/bootstrap.js" type="text/javascript"></script>
</head>
<body>
<script type="text/javascript">
    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
  m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

    ga('create', 'UA-62994246-1', 'auto');
    ga('send', 'pageview');

</script> 

<!--Accessibility feature to skip Navigation-->
<div id="skipNav"> <a href="#prof-main" class="skip">Skip navigation</a> </div>
<!--skipNav-->

<form id="mainForm" runat="server">
  <div id="wrapper-inner">
    <usr:nciheader runat="server" />
    <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
      <usr:header runat="server" />
      <usr:mainNav runat="server" />
      <div id="cedcd-main-content" class="row">
        <usr:contact runat="server" />

        <div id="prof-main">
          <div id="prof-header">
            <a class="back" href="#"><span class="glyphicon glyphicon-chevron-left"></span><span>Back to previous page</span></a>
            <h2 class="pg-title"><span runat="server" id="cd_name" /> (<span runat="server" id="cd_acronym" />)</h2>
            <div class="rightLink"> <span class="lastUpdated">Last Updated: <span runat="server" id="cd_lastupdate" /></span> </div>
            <div id="cd_errorMsg" class="errorText" runat="server"></div>
          </div>
          <!--prof-header-->
          
          <div class="row topMatter">
            <div class="cohortInfo col-md-6">
              <h3>Cohort Collaboration Contact</h3>
              <p class="profile-contact-intro" style="font-style:italic;font-size:.80em;">If interested in collaborating with the cohort on a project, please contact:</p>
              <ul runat="server" id="cd_contact" />
              
            </div>
            <!--cohortInfo col 1-->
            <div class="cohortInfo col-md-6 last">
              <h3>Principal Investigators</h3>
              <ul runat="server" id="piList" />
              
              <a runat="server" id="cd_website" class="bttn_submit" target="_blank"/> </div>
            <!--cohortInfo col 2--> 
          </div>
          <div class="row bottomMatter">
            <div id="attachments" runat="server" class="cohortInfo col-md-12">
              <button type="button" class="cedcd-btn active" aria-expanded="true" aria-controls="more"><span class="triangle"></span>Cohort Description</button>
              <div class="cohortInfoBody" id="more" aria-hidden="false" style="display: block;">
                <div runat="server" id="cd_description">
                  <ul>
                    <li>Pending</li>
                  </ul>
                  <!-- default text --> 
                </div>
              </div>
              <!--info-cat-body 1-->
              
              <button type="button" class="cedcd-btn" aria-expanded="false" aria-controls="protocols"><span class="triangle"></span>Protocols and Questionnaires</button>
              <div class="cohortInfoBody" id="protocols" aria-hidden="true">
                <h3>Study Protocol</h3>
                <div runat="server" id="prot_attachments">
                  <ul>
                    <li>Not Provided</li>
                  </ul>
                  <!-- default text --> 
                </div>
                <h3>Cohort Questionnaires</h3>
                <div runat="server" id="quest_attachments">
                  <ul>
                    <li>Not Provided</li>
                  </ul>
                  <!-- default text --> 
                </div>
              </div>
              <!--info-cat-body 1-->
              
              <button type="button" class="cedcd-btn" aria-expanded="false" aria-controls="policies"><span class="triangle"></span>Data, Biospecimen, and Authorship Policies</button>
              <div class="cohortInfoBody" id="policies" aria-hidden="true">
                <div runat="server" id="pol_attachments">
                  <ul>
                    <li>Not Provided</li>
                  </ul>
                  <!-- default text --> 
                </div>
              </div>
            </div>
            <!--info-cat--> 
          </div>
          <!-- prof-main --> 
        </div>
        <!-- main-content --> 
      </div>
      <usr:footer runat="server" />
    </div>
    <!--cedcd-bg-wrapper--> 
  </div>
  <!-- wrapper-inner --> 
  
  <!--javascript--> 
  <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script> 
  <script src="/scripts/westat.js" type="text/javascript"></script> 
  <script src="/scripts/jquery.sticky.js" type="text/javascript"></script> 
  <script type="text/javascript">
    $(function () { $('[data-toggle="tooltip"]').tooltip() })
    $(function () { $('[data-toggle="popover"]').popover() })
</script>
</form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>
