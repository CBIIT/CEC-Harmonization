<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="error.aspx.cs" Inherits="cec_publicweb.error" %>
<%@ Register Src="~/usrctrls/footer.ascx" TagName="footer" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/header.ascx" TagName="header" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/nci-header.ascx" TagName="nciheader" TagPrefix="usr" %>
<%@ Register Src="~/usrctrls/mainbarNav.ascx" TagName="mainNav" TagPrefix="usr" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en-us">
<head runat="server">
  <title>Cancer Epidemiology Descriptive Cohort Database (CEDCD)</title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge" />

  <!-- style links -->
  <link href="/css/bootstrap-tour.css" rel="stylesheet" type="text/css" />
  <link href="/css/bootstrap.css" rel="Stylesheet" type="text/css" />
  <link href="/css/main.css" rel="stylesheet" type="text/css" />

  <!-- web fonts -->
	<link href='https://fonts.googleapis.com/css?family=PT+Sans:400,700' rel='stylesheet' type='text/css' />
	
	<!--javascript-->
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" type="text/javascript"></script>
  <script src="/scripts/js_omni.js" language="javascript" type="text/javascript"></script>
  <script src="/scripts/bootstrap.js" language="javascript" type="text/javascript"></script>
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

<form runat="server">
  <div id="wrapper-inner">
    <usr:nciheader runat="server" />
    <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
      <usr:header runat="server" />
      <usr:mainNav runat="server" />
      <!-- MAIN CONTENT ----------------------------------------------------->
      <div id="cedcd-main-content" class="row clearfix">
        <div class="errors col-md-12">
          <h2>Error Encountered</h2>

          <!-- Session Invalid Error -------------------------------------------->
          <div id="error_sessioninvalid" runat="server" visible="false" >
            <p>You must be logged in to access this page. You will be redirected to the login page in 5 seconds. 
                  If you are not automatically redirected, please login at <a href='/select.aspx'>home.aspx</a> to continue.</p>

                  <script type="text/javascript">
                      redirectToURLInFiveSeconds('/select.aspx');
                  </script>  
            </div>
          <!-- End Session Invalid Error --------------------------------------->

          <!-- Account Lockout Error ------------------------------------------->
          <div id="error_accountlockout" runat="server" visible="false" >
            <p>Your account has been locked out due to consecutive invalid login attempts. In order to reset your account, please send an email to <a href="mailto:cedcdhelpdesk@westat.com">cedcdhelpdesk@westat.com</a></p>
          </div>
          <!-- End Account Lockout Error ------------------------------------------->

          <p id="simpleError" runat="server" />
        </div>
      </div><!--cedcd-main-content-->

      <usr:footer runat="server" />

    </div><!--cedcd-bg-wrapper-->
  </div><!-- wrapper-inner -->

    <!--javascript-->
    <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script>
    <script src="/scripts/westat.js" type="text/javascript"></script>
    <script src="/scripts/jquery.sticky.js" type="text/javascript"></script>
</form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>

</body>
</html>
