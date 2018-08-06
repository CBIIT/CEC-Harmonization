<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="forgotmyself.aspx.cs" Inherits="cec_publicweb.forgotmyself" %>
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
	<div id="skipNav">
		<a href="#fg-main" class="skip">Skip navigation</a>
    </div><!--skipNav-->

<form runat="server" onkeypress="javascript:submitOnEnter(event);">

    <div id="wrapper-inner">
        <usr:nciheader runat="server" />
        <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
            <usr:header runat="server" />
            <usr:mainNav runat="server" />

            <!-- MAIN CONTENT --------------------------------------------------- -->
            <div id="cedcd-main-content" class="row">
                <usr:contact runat="server" />

                <div id="fg-main" class="clearfix">
                <div id="fg-header" class="col-md-12">
			        <h2 class="pg-title">Forgot My Password</h2>
                </div> <!-- fg-header -->

                <div id="fg-col-1" class="col contact-col col-sm-4">
                    
                    <p runat="server" id="fg_errorMsg" class="bg-danger" />

                    <label for="fg_email" class="oneLineLabel">Email</label>
                    <asp:TextBox ID="fg_email" runat="server" />

                    <div class="bttn-group">
                        <asp:LinkButton ID="fg_sendBtn" runat="server" OnClick="forgotPassword_SendBtnClicked" Text="Submit" CssClass="bttn_submit" />
                        <asp:LinkButton ID="fg_cancelBtn" runat="server" OnClick="forgotPassword_CancelBtnClicked" Text="Cancel" CssClass="bttn_cancel" />
                        
                    </div>             
                </div> <!-- fg-col-1 -->

                <div id="fg-col-2" class="col contact-col col-sm-5">
                    <p>Check you email for instructions on how to change you password.</p>
                </div><!--fg-col-2-->

                </div> <!-- fg-main -->

            </div><!--cedcd-main-content-->
            <div class="clearFix"></div>

            <usr:footer runat="server" />
            <div class="clearFix"></div>  

        </div><!--cedcd-bg-wrapper--> 
     </div><!-- wrapper-inner -->

     <!--javascript-->
    <script src="/scripts/onmediaquery.min.js" type="text/javascript"></script>
    <script src="/scripts/westat.js" type="text/javascript"></script>
	<script src="/scripts/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">

          function submitOnEnter(e) {
              if (e && e.keyCode == 13) {
                  __doPostBack('fg_sendBtn', '');
              }
          }

  </script>
</form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>

</body>
</html>
