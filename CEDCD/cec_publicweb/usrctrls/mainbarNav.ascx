<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="mainbarNav.ascx.cs" Inherits="cec_publicweb.usrctrls.mainbarNav" %>

<div id="mainNavBar">
  <div id="mainNavBar-inner">
    <ul id="mainNav">
      <li runat="server" id="liHome"><a href="/select.aspx"><span>Home</span></a><span class="arrow down"></span></li>
      <li runat="server" id="liSelect"><a href="/cohortselect.aspx"><span>Cohort Details</span></a><span class="arrow down"></span></li>
      <li runat="server" id="liEnrollment"><a href="/enrollment.aspx"><span>Enrollment Counts</span></a><span class="arrow down"></span></li>
      <li runat="server" id="liCancer"><a href="/cancer.aspx"><span>Cancer Counts</span></a><span class="arrow down"></span></li>
      <li runat="server" id="liBiospecimen"><a href="/biospecimen.aspx"><span>Biospecimen Counts</span></a><span class="arrow down"></span></li>
      <li runat="server" id="liAbout"><a href="/about.aspx"><span>About</span></a><span class="arrow down"></span></li>
      <li runat="server" id="liLogin"><a data-toggle="modal" data-target="#login_dialog"><span>Cohort Login</span></a><span class="arrow down"></span></li>
      <li runat="server" id="liHelp"><a onclick="(function() { tour.init(); tour.restart(); })()"><span class="text-hide">Help</span><span class="glyphicon glyphicon-question-sign"></span></a></li>
    </ul>
  </div>
</div>
<input type="hidden" id="userToken" value="" runat="server" />


<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="login_dialog">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
        <div class="modal-header">
          <h2 class="modal-title">Login</h2>
        </div>
      <div class="modal-body">
        <p class="bg-danger text-danger" id="invalidlogin_msg"></p>
        <div class="form-group">
          <label for="username" class="control-label">Username</label>
          <input type="text" class="form-control" id="usernameIn" placeholder="Username" runat="server" enableviewstate="false">
        </div>
        <div class="form-group clearfix">
          <label for="password" class="control-label">Password</label>
          <input type="password" class="form-control" id="passwordIn" placeholder="Password" runat="server" enableviewstate="false">
          <p class="forgot"><a href="/forgotmyself.aspx">Forgot Password</a></p>
          <div class="pull-right">
            <button type="button" class="btn btn-default" data-dismiss="modal" onclick="reset_login_dialog_fields();">Cancel</button> <!-- -->
            <asp:Button class="btn btn-primary" ID="loginBtn" runat="server" Text="Login" />
          </div>
        </div>
          <div class="gov-notice">
            <p>This warning banner provides privacy and security notices consistent with applicable federal laws, directives, and other federal guidance for accessing this Government system, which includes (1) this computer network, (2) all computers connected to this network, and (3) all devices and storage media attached to this network or to a computer on this network.</p>
            <p>This system is provided for Government-authorized use only.</p>
            <p>Unauthorized or improper use of this system is prohibited and may result in disciplinary action and/or civil and criminal penalties.
                Personal use of social media and networking sites on this system is limited as to not interfere with official work duties and is subject to monitoring.</p>
            <p>By using this system, you understand and consent to the following:</p>
            <p>The Government may monitor, record, and audit your system usage, including usage of personal devices and email systems for official duties or to conduct HHS business. Therefore, you have no reasonable expectation of privacy regarding any communication or data transiting or stored on this system. At any time, and for any lawful Government purpose, the government may monitor, intercept, and search and seize any communication or data transiting or stored on this system.</p>
            <p>Any communication or data transiting or stored on this system may be disclosed or used for any lawful Government purpose.</p>
          </div>
        </div>
        <div class="modal-footer">
        </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->

<!-- session timeout modal -->
<div class="modal fade modal-straight-corners modal-cedcd-default" tabindex="-1" role="dialog" id="session_timingout_dialog">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title"><span class="glyphicon glyphicon-hourglass"></span> Session Timeout</h2>
        </div>
        <div class="modal-body">
            <div class="modal-text">
                <p>Your current session will expire in less than <b>5 minutes</b>. If you wish to extend your session, please click Continue.</p>
            </div>            
            <div class="pull-right">
                <button type="button" class="btn btn-default" onClick="__doPostBack('', '');">Continue</button>
            </div>
        </div>
        <div class="modal-footer">

        </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal end save modal -->


<script src="/scripts/sha256.js" type="text/javascript"></script>
<script src="/input/scripts/input_omni.js" type="text/javascript"></script>
<script type="text/javascript">

    function reset_login_dialog_fields() {
        location.href = '/select.aspx';
    }

    $('#login_dialog').on('show.bs.modal', function (event) {
        if ($('#invalidlogin_msg').text().length > 0) {
            $('#invalidlogin_msg').show();
        }
        else
            $('#invalidlogin_msg').hide();
    });

    $('#login_dialog').on('keypress', function (event) {
        if (event && event.keyCode == 13) {
            __doPostBack($('#loginBtn').attr('name'), '');
        }
    });

    var websrv = new CECInputFormService();
    function logOut() {
        websrv.processUserLogout($.parseJSON($('#userToken').val()), function (resp) {
            if (resp['error']) {
                console.warn(resp['error']['message']);
                alert(resp['error']['message']);
            }
            else {
                reset_login_dialog_fields();
                setCookie("uid", "");
                setCookie("sessionid", "");
                setCookie("edit_mode", "");
                setCookie(".ASPXAUTH", "");
                location.href = '/select.aspx?logout';
            }
        });
    }

    $(function () {
        if (getCookie("sessionid") !== "") {
            setInterval(function () { if (getCookie("edit_mode") == "edit") { saveCohortForm(false); } $('#session_timingout_dialog').modal('show') }, 900000);
        }
    });

</script>