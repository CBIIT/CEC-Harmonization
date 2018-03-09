<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="accessibility.aspx.cs" Inherits="cec_publicweb.privacy" %>
<%@ Register Src="~/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
<head runat="server">
    <title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />

    <!-- style links -->
    <link rel="Stylesheet" type="text/css" href="./css/main.css" />
    
    <!-- javascript -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
    <script src="/scripts/js_omni.js" type="text/javascript"></script>
    <script src="/scripts/bootstrap.js" type="text/javascript"></script>
</head>
<body>

<!--Accessibility feature to skip Navigation-->
	<div id="skipNav">
		<a href="#mainContentAccess" class="skip">Skip navigation</a>
    </div><!--skipNav-->

    <div id="header">
        <usr:header runat="server" />
    </div>
    <form runat="server">
    <div id="contentMain">
        <usr:mainNav runat="server" />

        <h4 id="mainContentAccess">Descriptive Cohort Database (DCD) Accessibility Policy</h4>

        <p>The Descriptive Cohort Database was designed to be accessible to all users and compatible with screen readers and other assistive technologies.  However, accessibility is an ongoing process.  Therefore, it is possible that you could encounter problems when accessing certain pages. If you would like to report a problem or obtain help accessing information on any Descriptive Cohort Database page, please send an <a href="mailto:null">email</a> to the Website Administrator.</p>

        <p>Please include the operating system of the computer you are using, the type and version of the device (e.g., PC or Mac), and title or URL of the page you are having difficulty accessing.</p>
   
    </div>

    <!--javascript-->
    <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script>
    <script src="/scripts/westat.js" type="text/javascript"></script>
	<script src="/scripts/jquery.sticky.js"></script>
    </form>

    
    <div id="footer">
             <usr:footer runat="server" />
    </div>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>
</body>
</html>
