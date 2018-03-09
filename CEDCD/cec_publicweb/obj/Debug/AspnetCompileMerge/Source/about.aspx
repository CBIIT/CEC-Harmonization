<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="about.aspx.cs" Inherits="cec_publicweb.about" %>
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
<div id="skipNav"> <a href="#about-main" class="skip">Skip navigation</a> </div>
<!--skipNav-->

<form runat="server">
  <div id="wrapper-inner">
    <usr:nciheader runat="server" />
    <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
      <usr:header runat="server" />
      <usr:mainNav runat="server" />
      
      <!-- MAIN CONTENT --------------------------------------------------- -->
      <div id="cedcd-main-content" class="row">
        <usr:contact runat="server" />

        <div id="about-main" class="clearfix">
          <div id="about-header" class="col-md-12">
            <h2 class="pg-title">About</h2>
          </div>
          <!--prof-header-->
          
          <div id="about-col-1" class="col-md-9 about-col">
            <h2>What the CEDCD is and its purpose</h2>
            <p>The Cancer Epidemiology Descriptive Cohort Database (CEDCD) contains descriptive information about cohort studies that follow groups of persons over time for cancer incidence, mortality, and other health outcomes.   The CEDCD is a searchable database that contains general study information (e.g., eligibility criteria and size), the type of data collected at baseline, cancer sites, number of participants diagnosed with cancer, and biospecimen information. All data included in this database are aggregated for each cohort; there are no individual level data. The goal of the CEDCD is to facilitate collaboration and highlight the opportunities for research within existing cohort studies.</p>
            <h2>How information is collected</h2>
            <p>Information in the CEDCD has been provided with approval from the cohort Principal Investigators (PIs). Cohort PIs may request correction of their information here: <a href="/contact.aspx"><u>contact NCI</u></a>. Annual updates are scheduled to begin in 2017. The date the data were collected/last updated is listed in the cohort description.  PIs who are interested in providing information about cohorts that are not currently included are encouraged to <a href="/contact.aspx"><u>contact NCI</u></a>.</p>
            <h2>Who we are</h2>
            <p>The CEDCD was developed through a contract with Westat and is maintained by the Epidemiology and Genomics Research Program (EGRP), located in the Division of Cancer Control and Population Sciences, National Cancer Institute's (NCI's), National Institutes of Health.</p>
            <p>EGRP supports scientific collaborations that pool the large quantity of available data and biospecimens in cohort studies for research in cancer etiology, prevention, and control. <a href="https://epi.grants.cancer.gov/" target="_blank"><u>Learn more about EGRP</u>.</a></p>
          </div>
          <!-- contact-col-1 -->
          
          <div id="about-col-2" class="col-md-3 about-col">
            <h2>Resource Links</h2>
            <h3>Useful Links</h3>
            <ul>
              <li><a href="https://epi.grants.cancer.gov/cohorts.html" target="_blank">EGRP Cancer Epidemiology Cohorts</a></li>
              <li><a href="https://epi.grants.cancer.gov/Consortia/cohort.html" target="_blank">NCI Cohort Consortium</a></li>
              <li><a href="http://epi.grants.cancer.gov/funding/" target="_blank">EGRP Funding &amp; Grants</a></li>
              <li><a href="https://biolincc.nhlbi.nih.gov/home/ " target="_blank">NHLBI BioLINCC</a></li>
              <li><a href="https://www.maelstrom-research.org/" target="_blank">Maelstrom</a></li>
              <li><a href="https://www.phenxtoolkit.org/index.php" target="_blank">PhenX Toolkit</a></li>
              <li><a href="https://www.bioshare.eu/" target="_blank">BioSHaRE</a></li>
              <li>Know of other related resources that should be included here? </li>
            </ul>
          </div>
          <!--contact-col-2--> 
        </div>
        <!-- contact-main --> 
      </div> 
      <!--cedcd-main-content-->
      <usr:footer runat="server" />
    </div>
    <!--cedcd-bg-wrapper--> 
  </div>
  <!-- wrapper-inner --> 
  
  <!--javascript--> 
  <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script> 
  <script src="/scripts/westat.js" type="text/javascript"></script> 
  <script src="/scripts/jquery.sticky.js"></script>
</form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>
