<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="userinfo.aspx.cs" Inherits="cec_publicweb.userinfo" %>
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
<body class="loggedin">    
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
		<a href="#usr-main" class="skip">Skip navigation</a>
    </div><!--skipNav-->

<form runat="server" >

    <div id="wrapper-inner">
        <usr:nciheader runat="server" />
        <div id="cedcd-bg-wrapper"> <!--for bg color/pattern-->
            <usr:header runat="server" />
            <usr:mainNav runat="server" />

            <!-- MAIN CONTENT --------------------------------------------------- -->
            <div id="cedcd-main-content" class="row">
                <usr:contact runat="server" />
              <div id="prof-header">
                <a class="back" href="#"><span class="glyphicon glyphicon-chevron-left"></span><span>Back to previous page</span></a>
              </div>

                <div id="usr-main" class="clearfix">
                    <div id="usr-header" class="col-md-12">
			            <h2 class="pg-title">My Account</h2>	            
		            </div><!---reg-header-->            

                    <div id="usr-col-1" class="col contact-col col-sm-4">

                        <p runat="server" id="rg_errorMsg" class="bg-danger"></p>

                        <div class="form-group">
                          <label class="oneLineLabel control-label" for="rg_password1">Password:</label>
                          <asp:TextBox class="form-control" ID="rg_password1" runat="server" TextMode="Password" />
                        </div>
                        <div class="form-group">
                          <label class="oneLineLabel control-label" for="rg_password2">Confirm Password:</label>
                          <asp:TextBox class="form-control" ID="rg_password2" runat="server" TextMode="Password" />
                        </div>

                        <div runat="server" id="userInformation">
                            <div class="form-group">
                                <label class="oneLineLabel control-label" for="rg_displayName">Display Name:</label>
                                <asp:TextBox ID="rg_displayName" class="form-control" runat="server" />
                            </div>
                            <div class="form-group">
                                <label class="oneLineLabel control-label" for="rg_emailAddress">Email:</label>
                                <asp:TextBox ID="rg_emailAddress" class="form-control" runat="server" />
                            </div>
                        </div>

                        <!-- captcha -->
                        <div class="form-group">
                          <label class="oneLineLabel control-label" for="rg_captcha">Solve Captcha: (<span id="captchaLabel" runat="server" />)<span class='required'>*</span></label>
                          <asp:TextBox class="form-control" ID="rg_captcha" runat="server" />
                        </div>

                        <div class="bttn-group">
                            <asp:LinkButton ID="rg_registerBtn" runat="server" OnClick="registrationBtnClicked" Text="Submit" CssClass="bttn_submit" />
                            <asp:LinkButton ID="fg_cancelBtn" runat="server" OnClick="CancelBtnClicked" Text="Cancel" CssClass="bttn_cancel" />
                        </div>
                    </div> <!-- usr-col-1 -->

                    <div id="usr-col-2" class="col contact-col col-sm-5">

						<h2>Password</h2>
						<ul class="first">
                        	<li>At least 8 characters </li>
                            <li>1 uppercase letter</li>
                            <li>1 lowercase letter</li>
                            <li>1 number</li>
                        </ul>
                        <p>Your password expires after 60 days, and you will be required to change it once you login. You cannot reuse any of your last six passwords.</p>

                        <h2>How do I delete my account from the system?</h2>
                        <p>Please send us a message with your request.</p>

                        <h2>Other questions or issues</h2>
                        <p>If you have any questions, comments or issues, please <a href="/contact.aspx" target="_self">send us a message</a>.</p>

                    </div><!--usr-col-2-->
                </div> <!-- usr-main -->

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
        
        $('#usr-main').on('keypress', function (e) {
            if (e && e.keyCode == 13) {
                __doPostBack('rg_registerBtn', '');
            }
        });

  </script></form>
<a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span class="glyphicon glyphicon-chevron-up"></span></a>

</body>
</html>